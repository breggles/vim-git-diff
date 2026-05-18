using System.Collections.Generic;

namespace VimGitDiff.Cli;

public sealed class CommandLineOptions
{
    public bool Cached { get; init; }

    public string? VimExecutable { get; init; }

    public string? ChangeDirectory { get; init; }

    public int? MaxFiles { get; init; }

    public bool All { get; init; }

    public bool Help { get; init; }

    public bool Version { get; init; }

    public IReadOnlyList<string> PositionalArgs { get; init; } = new List<string>();

    public IReadOnlyList<string>? ExplicitPaths { get; init; }
}
