using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Dto
{
    public class ScanRequestDto
    {
        public string CloneUrl { get; set; } = "";
        public string Branch { get; set; } = "main";
        public string? Token { get; set; }
        public string? RepoOwner { get; set; }
        public string? RepoName { get; set; } = "";
    }
}
