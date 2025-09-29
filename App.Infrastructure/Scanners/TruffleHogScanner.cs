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
    public class TruffleHogScanner
    {
        //public async Task<IEnumerable<Finding>> ScanAsync(string path)
        //{
        //    var json = await ProcessRunner.RunProcessCaptureAsync("trufflehog", $"filesystem {path} --json");
        //    var results = new List<Finding>();
        //    try
        //    {
        //        var doc = JsonDocument.Parse(json);
        //        if (doc.RootElement.ValueKind == JsonValueKind.Object)
        //        {
        //            // trufflehog v3 writes multiple json objects separated by newline; we may need to split
        //            // quick approach: split by newline and parse each line
        //            foreach (var line in json.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        //            {
        //                var el = JsonDocument.Parse(line).RootElement;
        //                var reason = el.TryGetProperty("reason", out var r) ? r.GetString() : "secret found";
        //                results.Add(new Finding
        //                {
        //                    Title = "Potential secret",
        //                    Severity = Severity.High,
        //                    FilePath = el.GetProperty("path").GetString() ?? "",
        //                    Evidence = line,
        //                    Scanner = "TruffleHog"
        //                });
        //            }
        //        }
        //    }
        //    catch { /* fallback: ignore parse errors */ }
        //    return results;
        //}
    }
}
