using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using VimGitDiff.Errors;

namespace VimGitDiff.Io;

public sealed class ProcessRunner : IProcessRunner
{
    public ProcessResult Run(string fileName, IReadOnlyList<string> arguments, string? workingDirectory)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };

        if (workingDirectory != null)
        {
            psi.WorkingDirectory = workingDirectory;
        }

        foreach (var arg in arguments)
        {
            psi.ArgumentList.Add(arg);
        }

        try
        {
            using var process = Process.Start(psi);

            if (process == null)
            {
                throw new VimGitDiffException($"failed to start process: {fileName}");
            }

            var stdout = process.StandardOutput.ReadToEnd();

            var stderr = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return new ProcessResult(process.ExitCode, stdout, stderr);
        }
        catch (FileNotFoundException ex)
        {
            throw new VimGitDiffException($"executable not found: {fileName}", ex);
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            throw new VimGitDiffException($"failed to start process: {fileName} ({ex.Message})", ex);
        }
    }
}
