using App.Api.Models;
using App.Application.Dto;
using App.Application.Interfaces.Services;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace App.Application.Service
{
    public class HipaaRiskService : IHipaaRiskService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly string _cacheFolder;
        private readonly string _openAiKey;
        private readonly bool _useAzure;
        private readonly string? _azureEndpoint;
        private readonly string? _azureDeployment;

        public HipaaRiskService(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _config = config;
            _cacheFolder = Path.Combine(AppContext.BaseDirectory, "hipaa_cache");
            Directory.CreateDirectory(_cacheFolder);

            _openAiKey = _config["OpenAI:ApiKey"] ?? "";
            _useAzure = bool.TryParse(_config["OpenAI:UseAzure"], out var b) && b;
            _azureEndpoint = _config["AzureOpenAI:Endpoint"];
            _azureDeployment = _config["AzureOpenAI:DeploymentName"];
        }

        public async Task<HipaaRiskResponse> AnalyzeRepoAsync(HipaaRiskRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.RepoUrl))
                throw new ArgumentException("RepoUrl is required", nameof(request));

            // 1) Clone repo into temp dir (or open if exists). Get latest commit sha.
            var repoPath = Path.Combine(Path.GetTempPath(), "ai_comp_repo", Guid.NewGuid().ToString("N"));
            Repository? repo = null;
            string commitSha = "";

            try
            {
                // Clone with LibGit2Sharp
                Repository.Clone(request.RepoUrl, repoPath);
                repo = new Repository(repoPath);
                commitSha = repo.Head.Tip.Sha;
            }
            catch (Exception ex)
            {
                // If clone fails, try shallow fallback or rethrow with helpful message
                throw new InvalidOperationException($"Failed to clone repository: {ex.Message}", ex);
            }

            // 2) Check cache (by commitSha)
            var cachePath = Path.Combine(_cacheFolder, $"{SanitizeFilename(request.RepoUrl)}_{commitSha}.json");
            if (!request.ForceRefresh && File.Exists(cachePath))
            {
                var cached = JsonConvert.DeserializeObject<HipaaRiskResponse>(File.ReadAllText(cachePath));
                if (cached != null)
                {
                    cached.GeneratedAt = DateTime.UtcNow;
                    cached.CommitSha = commitSha;
                    cached.RepoUrl = request.RepoUrl;
                    return cached;
                }
            }

            // 3) Lightweight candidate selection: collect code files likely to contain privacy/security issues
            var candidateFiles = CollectCandidateFiles(repoPath);

            // 4) For each candidate, prepare a compact payload (file path + snippet). We'll batch to avoid too many API calls.
            var fileSnippets = candidateFiles
    .Select(fp => new RepoFile { File = fp, Content = ReadFirstNLines(fp, 200) })
    .ToList();


            // 5) Create a prompt and call LLM
            var findings = await QueryLlmForFindingsAsync(request.RepoUrl, commitSha, fileSnippets, ct);

            // 6) Persist to cache and return
            var response = new HipaaRiskResponse
            {
                RepoUrl = request.RepoUrl,
                CommitSha = commitSha,
                GeneratedAt = DateTime.UtcNow,
                Risks = findings
            };

            File.WriteAllText(cachePath, JsonConvert.SerializeObject(response, Formatting.Indented));
            SafeDeleteDirectory(repoPath); // cleanup clone

            return response;
        }

        private List<RiskFinding> ParseLlmResponseToFindings(string llmJson)
        {
            // We expect structured json like: { "risks": [ { "file":"...", "issue":"...", "evidence":"...", "severity":"High" } ] }
            try
            {
                var root = JsonConvert.DeserializeObject<Dictionary<string, object>>(llmJson);
                if (root != null && root.TryGetValue("risks", out var risksObj))
                {
                    var arr = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(risksObj.ToString() ?? "[]");
                    var list = new List<RiskFinding>();
                    if (arr != null)
                    {
                        foreach (var item in arr)
                        {
                            list.Add(new RiskFinding
                            {
                                File = item.GetValueOrDefault("file") ?? item.GetValueOrDefault("path") ?? "",
                                Issue = item.GetValueOrDefault("issue") ?? item.GetValueOrDefault("finding") ?? "",
                                Evidence = item.GetValueOrDefault("evidence") ?? "",
                                Severity = item.GetValueOrDefault("severity") ?? ""
                            });
                        }
                    }
                    return list;
                }
            }
            catch
            {
                // best-effort fallback: try to parse any lines "FILE: ... - ISSUE: ..."
            }
            return new List<RiskFinding>();
        }

        private async Task<List<RiskFinding>> QueryLlmForFindingsAsync(
    string repoUrl, string commitSha, List<RepoFile> fileSnippets, CancellationToken ct)

        {
            var client = _httpFactory.CreateClient();
            // prepare a short system + user prompt that instructs the LLM to output JSON
            var systemPrompt = @"
You are a security & compliance assistant. For each file snippet provided, identify whether it represents a HIPAA compliance risk for a healthcare application repository. 
Return a single JSON object with a `risks` array. Each array item must contain: file, issue (one-line), evidence (short phrase from snippet), severity (Low/Medium/High). 
Only include genuine risks. If none found, return { ""risks"": [] }.
";

            // Build message with a few candidates (batch to avoid token limits). We'll include up to N files per request; if many files, call in multiple batches.
            const int batchSize = 8;
            var allFindings = new List<RiskFinding>();
            for (int i = 0; i < fileSnippets.Count; i += batchSize)
            {
                var batch = fileSnippets.Skip(i).Take(batchSize).ToList();
                var userBuilder = new StringBuilder();
                userBuilder.AppendLine($"Repo: {repoUrl}");
                userBuilder.AppendLine($"Commit: {commitSha}");
                userBuilder.AppendLine("Analyze the following file snippets for HIPAA compliance risk. Only output valid JSON structure as described.");
                userBuilder.AppendLine();
                int idx = 1;
                foreach (var f in batch)
                {
                    userBuilder.AppendLine($"##### FILE {idx}: {f.File}");
                    userBuilder.AppendLine("```");
                    userBuilder.AppendLine(f.Content);
                    userBuilder.AppendLine("```");
                    userBuilder.AppendLine();
                    idx++;
                }

                var userPrompt = userBuilder.ToString();

                // Call OpenAI/Azure
                var llmJson = await CallChatApiAsync(systemPrompt, userPrompt, ct);
                if (!string.IsNullOrWhiteSpace(llmJson))
                {
                    var parsed = ParseLlmResponseToFindings(llmJson);
                    allFindings.AddRange(parsed);
                }
            }

            // de-duplicate by file+issue
            var dedup = allFindings
                .GroupBy(r => (r.File, r.Issue))
                .Select(g => g.First())
                .ToList();

            return dedup;
        }

        private async Task<string> CallChatApiAsync(string systemPrompt, string userPrompt, CancellationToken ct)
        {
            // This method supports two modes:
            //  - Public OpenAI API (api.openai.com) if UseAzure == false
            //  - Azure OpenAI REST endpoint if UseAzure == true (endpoint + deployment)
            // We'll call the chat completions endpoint and attempt to extract assistant.content which should be JSON.

            var http = _httpFactory.CreateClient();
            http.Timeout = TimeSpan.FromSeconds(120);

            if (!_useAzure)
            {
                // OpenAI public endpoint
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _openAiKey);

                var payload = new
                {
                    model = "gpt-4o-mini", // you can change to gpt-4o or gpt-3.5-turbo if limited
                    messages = new object[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    max_tokens = 800,
                    temperature = 0.0
                };

                var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                };

                var res = await http.SendAsync(req, ct);
                res.EnsureSuccessStatusCode();
                var body = await res.Content.ReadAsStringAsync(ct);
                // parse assistant content
                dynamic root = JsonConvert.DeserializeObject(body)!;
                try
                {
                    var content = root.choices[0].message.content.ToString();
                    return content;
                }
                catch
                {
                    return body;
                }
            }
            else
            {
                // Azure OpenAI: Post to {endpoint}/openai/deployments/{deployment}/chat/completions?api-version=2023-10-01-preview
                if (string.IsNullOrWhiteSpace(_azureEndpoint) || string.IsNullOrWhiteSpace(_azureDeployment))
                    throw new InvalidOperationException("Azure OpenAI configuration missing.");

                http.DefaultRequestHeaders.Add("api-key", _openAiKey);

                var payload = new
                {
                    messages = new object[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    max_tokens = 800,
                    temperature = 0.0
                };

                var uri = $"{_azureEndpoint}/openai/deployments/{_azureDeployment}/chat/completions?api-version=2023-10-01-preview";
                var req = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                };
                var res = await http.SendAsync(req, ct);
                res.EnsureSuccessStatusCode();
                var body = await res.Content.ReadAsStringAsync(ct);
                dynamic root = JsonConvert.DeserializeObject(body)!;
                try
                {
                    var content = root.choices[0].message.content.ToString();
                    return content;
                }
                catch
                {
                    return body;
                }
            }
        }

        private static string ReadFirstNLines(string path, int charLimit = 2000)
        {
            try
            {
                var txt = File.ReadAllText(path);
                if (txt.Length <= charLimit) return txt;
                return txt.Substring(0, charLimit) + "\n... [truncated]";
            }
            catch { return ""; }
        }

        private static List<string> CollectCandidateFiles(string root)
        {
            // quick heuristics: check files that often contain PHI handling or secrets
            var exts = new[] { ".cs", ".cshtml", ".config", ".json", ".js", ".ts", ".py", ".java", ".sql" };
            var candidates = Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories)
                .Where(f => exts.Contains(Path.GetExtension(f).ToLowerInvariant()))
                // exclude large vendor folders
                .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}") &&
                            !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") &&
                            !f.Contains($"{Path.DirectorySeparatorChar}node_modules{Path.DirectorySeparatorChar}"))
                .ToList();

            // further filter by keywords to reduce payload size
            var keywords = new[] { "patient", "health", "ssn", "dob", "socialsecurity", "medical", "PHI", "password", "connectionstring", "api_key", "log", "db", "sensitive", "encrypt", "tls", "https" };
            var filtered = new List<string>();
            foreach (var f in candidates)
            {
                try
                {
                    var text = File.ReadAllText(f).ToLowerInvariant();
                    if (keywords.Any(k => text.Contains(k)))
                    {
                        filtered.Add(f);
                        continue;
                    }
                    // also include small controllers & services by path heuristics
                    var name = Path.GetFileName(f).ToLowerInvariant();
                    if (name.Contains("controller") || name.Contains("service") || name.Contains("repository"))
                        filtered.Add(f);
                }
                catch { }
            }

            // if no filtered files, fall back to several top files
            if (!filtered.Any())
            {
                filtered = candidates.Take(50).ToList();
            }

            return filtered;
        }

        private static string SanitizeFilename(string s)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                s = s.Replace(c, '_');
            return s.Replace("://", "__").Replace("/", "_");
        }

        private static void SafeDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch { /* ignore cleanup errors */ }
        }
    }
}
