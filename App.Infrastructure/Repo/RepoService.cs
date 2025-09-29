using LibGit2Sharp;
using System;
using System.IO;
namespace App.Infrastructure.Repo
{
    public class RepoService
    {
        //public string CloneRepo(string repoUrl, string branch)
        //{
        //    var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        //    LibGit2Sharp.Repository.Clone(repoUrl, tempPath, new LibGit2Sharp.CloneOptions
        //    {
        //        BranchName = branch
        //    });

        //    return tempPath;
        //}
        public string CloneRepo(string repoUrl, string branch, string? token = null)
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            if (!string.IsNullOrEmpty(token))
            {
                // Private repo with token
                Repository.Clone(
                    repoUrl,
                    path,
                    new CloneOptions
                    {
                        BranchName = branch,
                        Checkout = true,
                        CredentialsProvider = (_url, _user, _cred) =>
                            new UsernamePasswordCredentials
                            {
                                Username = token,
                                Password = ""
                            }
                    }
                );
            }
            else
            {
                // Public repo, no credentials
                Repository.Clone(repoUrl, path, new CloneOptions
                {
                    BranchName = branch,
                    Checkout = true
                });
            }

            return path;
        }

    }
}
