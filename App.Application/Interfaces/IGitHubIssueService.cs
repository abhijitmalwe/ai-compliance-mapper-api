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

    }
}
