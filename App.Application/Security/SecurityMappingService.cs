using App.Application.Dto;
using App.Application.Interfaces;
using App.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Security
{
    public class SecurityMappingService : ISecurityMappingService
    {
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
