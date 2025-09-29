using App.Domain.Entities;
using App.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Scanners
{
    public class ZapScanner
    {
        public List<Finding> Scan(string repoLocalPath)
        {
            // For PoC return a single simulated finding. Replace with real CLI tool calls.
            var f = new Finding
            {
                Title = "Hardcoded DB password in appsettings.json",
                Description = "Found a likely hardcoded connection string with password in appsettings.json.",
                Evidence = "\"ConnectionStrings:Default\": \"Server=...;Password=pa$$w0rd;\"",
                Severity = Severity.High,
                Scanner = "Simulated"
            };

            return new List<Finding> { f };
        }
    }
}
