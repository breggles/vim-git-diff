using System.Collections.Generic;

namespace VimGitDiff.Io;

public interface IProcessRunner
{
    ProcessResult Run(string fileName, IReadOnlyList<string> arguments, string? workingDirectory);
}
