using VimGitDiff.Errors;

namespace VimGitDiff.Git;

public sealed class GitRepository
{
    public string Root { get; }

    private GitRepository(string root)
    {
        Root = root;
    }

    public static GitRepository Discover(IGitRunner git, string startDirectory)
    {
        var result = git.Run(new[] { "rev-parse", "--show-toplevel" }, startDirectory);

        if (result.ExitCode != 0)
        {
            throw new NotAGitRepoException($"{startDirectory} is not inside a git repository");
        }

        var root = result.StdOut.Trim();

        if (root.Length == 0)
        {
            throw new NotAGitRepoException($"{startDirectory} is not inside a git repository");
        }

        return new GitRepository(root);
    }
}
