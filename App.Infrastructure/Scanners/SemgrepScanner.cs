using App.Domain.Entities;
using App.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Infrastructure.Scanners
{
    public class SemgrepScanner
    {
        //public async Task<IEnumerable<Finding>> ScanAsync(string path)
        //{
        //    // semgrep --config p/owasp-top-ten --json -o out.json <path>
        //    var json = await ProcessRunner.RunProcessCaptureAsync("semgrep", $"--config p/owasp-top-ten --json {path}");
        //    // parse JSON into a lightweight model and map to Finding
        //    var doc = JsonDocument.Parse(json);
        //    var results = new List<Finding>();
        //    if (doc.RootElement.TryGetProperty("results", out var arr))
        //    {
        //        foreach (var item in arr.EnumerateArray())
        //        {
        //            var message = item.GetProperty("extra").GetProperty("message").GetString() ?? "semgrep issue";
        //            var pathStr = item.GetProperty("path").GetString() ?? "";
        //            var startLine = item.GetProperty("start").GetProperty("line").GetInt32();
        //            results.Add(new Finding
        //            {
        //                Title = message,
        //                Severity = Severity.Medium, // default; we will refine later
        //                FilePath = pathStr,
        //                Line = startLine,
        //                Evidence = item.ToString(),
        //                Scanner = "Semgrep"
        //            });
        //        }
        //    }
        //    return results;
        //}
    }
}
