using System.Reflection;

namespace VimGitDiff.Cli;

public static class VersionInfo
{
    public static string Format(string? informationalVersion)
    {
        if (string.IsNullOrEmpty(informationalVersion))
        {
            return "unknown";
        }

        var plus = informationalVersion.IndexOf('+');

        return plus < 0 ? informationalVersion : informationalVersion.Substring(0, plus);
    }

    public static string Current()
    {
        var attribute = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        return Format(attribute?.InformationalVersion);
    }
}
