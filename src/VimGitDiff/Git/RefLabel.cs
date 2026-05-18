namespace VimGitDiff.Git;

public static class RefLabel
{
    public static string For(string @ref)
    {
        if (@ref == GitRefSpec.WorkingTree)
        {
            return "working tree";
        }

        if (@ref == GitRefSpec.Index)
        {
            return "index";
        }

        return @ref;
    }
}
