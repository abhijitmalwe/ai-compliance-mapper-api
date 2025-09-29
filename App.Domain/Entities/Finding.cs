using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities
{
    public enum Severity { Info, Low, Medium, High, Critical }
    public class Finding
    {
        //public string Id { get; set; } = Guid.NewGuid().ToString();
        //public string Title { get; set; } = "";
        //public string Description { get; set; }
        //public Severity Severity { get; set; }
        //public string FilePath { get; set; } = "";
        //public int? Line { get; set; }
        //public string Evidence { get; set; } = "";
        //public string Scanner { get; set; } = ""; // e.g. "Semgrep", "TruffleHog", "Roslyn", "ZAP"
        //public string? ComplianceControl { get; set; } // mapped HIPAA/OWASP control
        //public string? Remediation { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public string Evidence { get; set; }

    }
}
