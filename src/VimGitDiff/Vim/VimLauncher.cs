using System.Diagnostics;

using VimGitDiff.Errors;

namespace VimGitDiff.Vim;

public sealed class VimLauncher
{
    public int Launch(string vimExecutable, string scriptPath)
    {
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

            process.WaitForExit();

            return process.ExitCode;
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
