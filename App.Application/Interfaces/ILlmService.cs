using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces
{
    public interface ILlmService
    {
        Task<string> AskAsync(string prompt, CancellationToken ct = default);

    }
}
