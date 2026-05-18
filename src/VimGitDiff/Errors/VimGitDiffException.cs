using System;

namespace VimGitDiff.Errors;

public class VimGitDiffException : Exception
{
    public VimGitDiffException(string message) : base(message)
    {
    }

    public VimGitDiffException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
