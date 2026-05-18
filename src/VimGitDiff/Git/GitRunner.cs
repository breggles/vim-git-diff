using System.Collections.Generic;

using VimGitDiff.Errors;
using VimGitDiff.Io;

namespace VimGitDiff.Git;

public sealed class GitRunner : IGitRunner
{
    private readonly IProcessRunner _processRunner;

    private readonly string _gitExecutable;

    public GitRunner(IProcessRunner processRunner, string gitExecutable = "git")
    {
        _processRunner = processRunner;

        _gitExecutable = gitExecutable;
    }

    public ProcessResult Run(IReadOnlyList<string> arguments, string? workingDirectory)
    {
        try
        {
            return _processRunner.Run(_gitExecutable, arguments, workingDirectory);
        }
        catch (VimGitDiffException ex)
        {
            throw new GitNotFoundException($"git is not available on PATH ({ex.Message})");
        }
    }
}
