namespace VimGitDiff.Io;

public sealed record ProcessResult(int ExitCode, string StdOut, string StdErr);
