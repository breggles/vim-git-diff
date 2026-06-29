using System;
using System.Diagnostics;

using VimGitDiff.Errors;

namespace VimGitDiff.Vim;

public readonly record struct LaunchResult(int ExitCode, bool Detached);

public sealed class VimLauncher
{
    public static bool IsGui(string vimExecutable)
    {
        var name = System.IO.Path.GetFileNameWithoutExtension(vimExecutable);

        return string.Equals(name, "gvim", StringComparison.OrdinalIgnoreCase);
    }

    public LaunchResult Launch(string vimExecutable, string scriptPath)
    {
        var detach = IsGui(vimExecutable);

        var psi = new ProcessStartInfo
        {
            FileName = vimExecutable,
            UseShellExecute = false,
        };

        psi.ArgumentList.Add("--cmd");

        psi.ArgumentList.Add("set noswapfile");

        psi.ArgumentList.Add("-S");

        psi.ArgumentList.Add(scriptPath);

        try
        {
            using var process = Process.Start(psi);

            if (process == null)
            {
                throw new VimNotFoundException($"failed to start {vimExecutable}");
            }

            if (detach)
            {
                return new LaunchResult(0, Detached: true);
            }

            process.WaitForExit();

            return new LaunchResult(process.ExitCode, Detached: false);
        }
        catch (System.IO.FileNotFoundException ex)
        {
            throw new VimNotFoundException($"vim executable not found: {vimExecutable}", ex);
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            throw new VimNotFoundException($"failed to start vim ({ex.Message})", ex);
        }
    }
}
