using App.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces
{
    public interface IGitHubIssueService
    {
        Task CreateIssueAsync(string owner, string repo, string title, string body);
        List<(string id, string title, string explain, string remediation)> MapFindingToHipaaControls(Finding finding);

    }
}
