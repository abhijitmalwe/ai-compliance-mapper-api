using App.Application.Interfaces;
using LibGit2Sharp;

namespace App.Infrastructure.Repo
{
    public class RepoService : IRepoScannerService
    {
        public string CloneRepo(string repoUrl, string branch, string? token = null)
        {
            // ensure repoUrl is https
            if (!repoUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only http/https repository URLs are supported for this PoC.");

            var workingDir = Path.Combine(Path.GetTempPath(), "compliance_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(workingDir);

            string urlToClone = repoUrl;
            if (!string.IsNullOrEmpty(token))
            {
                // inject token into URL (for demo purposes only)
                if (repoUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    urlToClone = "https://" + token + "@" + repoUrl.Substring("https://".Length);
                }
            }

            // clone (no special credentials API used in LibGit2Sharp 0.31.0)
            var cloneOptions = new CloneOptions { BranchName = branch, Checkout = true };
            LibGit2Sharp.Repository.Clone(urlToClone, workingDir, cloneOptions);

            return workingDir;
        }
    }
}
