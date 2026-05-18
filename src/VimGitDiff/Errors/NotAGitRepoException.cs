namespace VimGitDiff.Errors;

public sealed class NotAGitRepoException : VimGitDiffException
{
    public NotAGitRepoException(string message) : base(message)
    {
    }
}
