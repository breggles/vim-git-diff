using System;
using System.IO;
using System.Runtime.InteropServices;

using VimGitDiff.Errors;

namespace VimGitDiff.Vim;

public static class VimDiscovery
{
    public const string EnvVar = "VIM_GIT_DIFF_VIM";

    public static string Resolve(string? explicitPath, Func<string, string?> envLookup)
    {
        if (!string.IsNullOrEmpty(explicitPath))
        {
            return explicitPath;
        }

        var fromEnv = envLookup(EnvVar);

        if (!string.IsNullOrEmpty(fromEnv))
        {
            return fromEnv;
        }

        var candidates = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new[] { "gvim.exe", "gvim", "vim.exe", "vim" }
            : new[] { "gvim", "vim" };

        foreach (var candidate in candidates)
        {
            if (FindOnPath(candidate, envLookup) is { } found)
            {
                return found;
            }
        }

        throw new VimNotFoundException("could not find gvim or vim on PATH; use --vim or set " + EnvVar);
    }

    private static string? FindOnPath(string name, Func<string, string?> envLookup)
    {
        var pathEnv = envLookup("PATH");

        if (string.IsNullOrEmpty(pathEnv))
        {
            return null;
        }

        var separator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':';

        foreach (var dir in pathEnv.Split(separator))
        {
            if (string.IsNullOrEmpty(dir))
            {
                continue;
            }

            var candidate = Path.Combine(dir, name);

            try
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        return null;
    }
}
