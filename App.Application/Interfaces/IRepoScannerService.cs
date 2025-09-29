using App.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces
{
    public interface IRepoScannerService
    {
        //Task<RepoSnapShot> CloneRepositoryAsync(string cloneUrl, string branch = "main");
        //Task<IReadOnlyList<Finding>> ScanRepositoryAsync(RepoSnapShot snapshot, CancellationToken ct = default);
        string CloneRepo(string repoUrl, string branch, string? token = null);
    }
}
