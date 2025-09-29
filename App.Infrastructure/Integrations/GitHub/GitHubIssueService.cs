using App.Application.Interfaces;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Integrations.GitHub
{
    public class GitHubIssueService: IGitHubIssueService
    {
        private readonly GitHubClient _client;

        public GitHubIssueService(string token)
        {
            _client = new GitHubClient(new ProductHeaderValue("ComplianceAgent"));
            _client.Credentials = new Credentials(token);
        }

        public async Task CreateIssueAsync(string owner, string repo, string title, string body)
        {
            var issue = new NewIssue(title) { Body = body };
            await _client.Issue.Create(owner, repo, issue);
        }
    }
}



