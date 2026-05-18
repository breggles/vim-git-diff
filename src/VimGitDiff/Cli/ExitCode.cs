namespace VimGitDiff.Cli;

public enum ExitCode
{
    Ok = 0,

    Other = 1,

    Usage = 2,

    NotAGitRepo = 3,

    BadRef = 4,

    GitMissing = 5,

    VimMissing = 6,
}
