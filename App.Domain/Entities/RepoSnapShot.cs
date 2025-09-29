using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities
{
    public class RepoSnapShot
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Owner { get; set; } = "";
        public string Repo { get; set; } = "";
        public string CloneUrl { get; set; } = "";
        public string CommitSha { get; set; } = "";
        public DateTimeOffset ScannedAt { get; set; } = DateTimeOffset.UtcNow;
        public string LocalPath { get; set; } = "";
    }
}
