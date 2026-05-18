namespace VimGitDiff.Git;

public static class GitRefClassifier
{
    public static bool IsCommitish(IGitRunner git, string repoRoot, string token)
    {
        if (token.StartsWith('-'))
        {
            return false;
        }

        var result = git.Run(new[] { "rev-parse", "--verify", "--quiet", token + "^{commit}" }, repoRoot);

        return result.ExitCode == 0;
    }
}
