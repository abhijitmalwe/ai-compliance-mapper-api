using App.Application.Interfaces;
using App.Domain.Entities;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Integrations.GitHub
{
    public class GitHubIssueService : IGitHubIssueService
    {
        private readonly GitHubClient _client;

        public GitHubIssueService(string token)
        {
            _client = new GitHubClient(new ProductHeaderValue("ComplianceAgent"));
            if (!string.IsNullOrEmpty(token))
                _client.Credentials = new Credentials(token);
        }

        public async Task CreateIssueAsync(string owner, string repo, string title, string body)
        {
            if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(repo))
                return; // skip if owner/repo not provided

            var newIssue = new NewIssue(title) { Body = body };
            await _client.Issue.Create(owner, repo, newIssue);
        }

        public List<(string id, string title, string explain, string remediation)> MapFindingToHipaaControls(Finding finding)
        {
            // Example mapping when hardcoded secrets found
            if (finding.Title.Contains("password", System.StringComparison.OrdinalIgnoreCase)
                || finding.Description.Contains("hardcoded", System.StringComparison.OrdinalIgnoreCase))
            {
                return new List<(string, string, string, string)>
                {
                    ("164.312(a)(1)", "Access Control",
                     "Hardcoded credentials can lead to unauthorized access to protected data.",
                     "Remove secrets from code. Use environment variables or a secrets manager (KeyVault).")
                };
            }

            // Default mapping
            return new List<(string, string, string, string)>
            {
                ("164.312(a)(2)", "Audit Controls",
                 "Review and logging may be required for this finding.",
                 "Apply logging and monitoring controls, follow least privilege.")
            };
        }

       

        
    }
}



