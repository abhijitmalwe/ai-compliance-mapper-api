using App.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Dto
{
    public class ScanResultDto
    {
        public string ScanId { get; set; } = Guid.NewGuid().ToString();
        public string Repo { get; set; } = "";
        public IList<Finding> Findings { get; set; } = new List<Finding>();
        public string? ReportBase64 { get; set; }
    }
}
