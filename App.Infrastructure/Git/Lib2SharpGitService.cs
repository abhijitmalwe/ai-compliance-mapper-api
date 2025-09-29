using App.Application.Interfaces;
using App.Domain.Entities;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Git
{
    public class Lib2SharpGitService
    {
        private readonly string _workspaceRoot;
        public Lib2SharpGitService(string workspaceRoot = "/tmp/compliance")
        {
            _workspaceRoot = workspaceRoot;
            Directory.CreateDirectory(_workspaceRoot);
        }

        //public string CloneRepo(string repoUrl, string branch)
        //{
        //    var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        //    var cloneOptions = new CloneOptions
        //    {
        //        BranchName = branch,
        //        Checkout = true
        //    };

        //    Repository.Clone(repoUrl, path, cloneOptions);
        //    return path; // return local path of cloned repo
        //}

        //public async Task<RepoSnapShot> CloneRepositoryAsync(string cloneUrl, string branch = "main")
        //{
        //    var ownerRepo = ParseOwnerRepoFromUrl(cloneUrl); // implement small parser
        //    var localPath = Path.Combine(_workspaceRoot, $"{ownerRepo.owner}_{ownerRepo.repo}_{Guid.NewGuid():N}");
        //    // Use LibGit2Sharp to clone
        //    var co = new CloneOptions { BranchName = branch };
        //    Repository.Clone(ownerRepo, localPath, co);
        //    var sha = new Repository(localPath).Head.Tip.Sha;

        //    return new RepoSnapShot
        //    {
        //        Owner = ownerRepo.owner,
        //        Repo = ownerRepo.repo,
        //        CloneUrl = cloneUrl,
        //        CommitSha = sha,
        //        LocalPath = localPath,
        //        ScannedAt = DateTimeOffset.UtcNow
        //    };
        //}

        //public async Task<IReadOnlyList<Finding>> ScanRepositoryAsync(RepoSnapShot snapshot, CancellationToken ct = default)
        //{
        //    // This service will orchestrate a set of scanner calls; for clarity, we'll call other infra services.
        //    throw new NotImplementedException("Use orchestrator service to call scanners.");
        //}

        //private (string owner, string repo) ParseOwnerRepoFromUrl(string url)
        //{
        //    // naive parser for https://github.com/owner/repo(.git)
        //    var uri = new Uri(url);
        //    var parts = uri.AbsolutePath.Trim('/').Split('/');
        //    return (parts[0], parts[1].Replace(".git", ""));
        //}
    }
}
