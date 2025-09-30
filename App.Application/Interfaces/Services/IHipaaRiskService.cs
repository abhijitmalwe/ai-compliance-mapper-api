using App.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Services
{
    public interface IHipaaRiskService
    {

        Task<HipaaRiskResponse> AnalyzeRepoAsync(HipaaRiskRequest request, CancellationToken ct = default);

    }
}
