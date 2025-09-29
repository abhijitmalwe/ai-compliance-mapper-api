using App.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces
{
    public interface ISecurityMappingService
    {
        List<HippaControlDto> MapFindingToHipaaControls(string finding);

    }
}
