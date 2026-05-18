namespace VimGitDiff.Errors;

public sealed class UsageException : VimGitDiffException
{
    public UsageException(string message) : base(message)
    {
    }
}
