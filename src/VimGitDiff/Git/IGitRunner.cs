using System.Collections.Generic;

using VimGitDiff.Io;

namespace VimGitDiff.Git;

public interface IGitRunner
{
    ProcessResult Run(IReadOnlyList<string> arguments, string? workingDirectory);
}
