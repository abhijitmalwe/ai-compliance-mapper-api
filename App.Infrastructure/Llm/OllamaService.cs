using App.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Infrastructure.Llm
{
    public class OllamaService:ILlmService
    {
        private readonly HttpClient _http;
        public OllamaService(HttpClient http) => _http = http;

        public async Task<string> AskAsync(string prompt, CancellationToken ct = default)
        {
            var req = new { model = "mistral", prompt = prompt };
            var resp = await _http.PostAsJsonAsync("/api/generate", req, ct);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            // adjust parsing to Ollama's response format
            return body.GetProperty("text").GetString() ?? "";
        }
    }
}
