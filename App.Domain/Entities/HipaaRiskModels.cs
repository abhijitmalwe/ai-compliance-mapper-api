namespace App.Api.Models
{

    public record HipaaRiskRequest(string RepoUrl, string Branch = "main", bool ForceRefresh = false);

    public class RiskFinding
    {
        public string File { get; set; } = "";
        public string Issue { get; set; } = "";
        public string Evidence { get; set; } = "";
        public string Severity { get; set; } = "";
    }

    public class HipaaRiskResponse
    {
        public List<RiskFinding> Risks { get; set; } = new List<RiskFinding>();
        public string RepoUrl { get; set; } = "";
        public string CommitSha { get; set; } = "";
        public DateTime GeneratedAt { get; set; }
    }
}

