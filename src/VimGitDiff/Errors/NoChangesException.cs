namespace VimGitDiff.Errors;

public sealed class NoChangesException : VimGitDiffException
{
    public NoChangesException(string message) : base(message)
    {
    }
}
