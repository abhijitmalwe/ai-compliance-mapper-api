using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Utils
{
    public static class ProcessRunner
    {
        public static async Task<string> RunProcessCaptureAsync(string fileName, string args, string workingDirectory = "", CancellationToken ct = default)
        {
            var psi = new ProcessStartInfo(fileName, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = string.IsNullOrEmpty(workingDirectory) ? Environment.CurrentDirectory : workingDirectory
            };

            using var p = Process.Start(psi)!;
            var output = await p.StandardOutput.ReadToEndAsync();
            var error = await p.StandardError.ReadToEndAsync();
            await p.WaitForExitAsync(ct);
            if (p.ExitCode != 0)
                Console.WriteLine($"Process {fileName} exit {p.ExitCode}: {error}");
            return output + "\n" + error;
        }
    }
}
