namespace VimGitDiff.Errors;

public sealed class BadRefException : VimGitDiffException
{
    public BadRefException(string message) : base(message)
    {
    }
}
