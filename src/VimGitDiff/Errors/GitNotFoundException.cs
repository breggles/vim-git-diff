namespace VimGitDiff.Errors;

public sealed class GitNotFoundException : VimGitDiffException
{
    public GitNotFoundException(string message) : base(message)
    {
    }
}
