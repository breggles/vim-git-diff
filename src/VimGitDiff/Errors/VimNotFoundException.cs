namespace VimGitDiff.Errors;

public sealed class VimNotFoundException : VimGitDiffException
{
    public VimNotFoundException(string message) : base(message)
    {
    }

    public VimNotFoundException(string message, System.Exception innerException) : base(message, innerException)
    {
    }
}
