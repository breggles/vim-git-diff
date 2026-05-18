using System.Collections.Generic;

using Xunit;

using VimGitDiff.Cli;
using VimGitDiff.Errors;
using VimGitDiff.Git;
using VimGitDiff.Io;
using VimGitDiff.Tests.Fakes;

namespace VimGitDiff.Tests.Cli;

public class RefAndPathResolverTests
{
    private static FakeGitRunner GitWhereCommitsAre(params string[] commitishTokens)
    {
        var set = new HashSet<string>();

        foreach (var t in commitishTokens)
        {
            set.Add(t + "^{commit}");
        }

        return new FakeGitRunner
        {
            Responder = (args, _) =>
            {
                if (args.Count == 4 && args[0] == "rev-parse" && args[1] == "--verify" && args[2] == "--quiet")
                {
                    return new ProcessResult(set.Contains(args[3]) ? 0 : 128, "", "");
                }

                return new ProcessResult(0, "", "");
            },
        };
    }

    [Fact]
    public void No_args_index_vs_working_tree()
    {
        var git = GitWhereCommitsAre();

        var opts = new CommandLineOptions();

        var resolved = RefAndPathResolver.Resolve(git, "/repo", opts);

        Assert.Equal(GitRefSpec.IndexVsWorkingTree, resolved.Refs);

        Assert.Empty(resolved.Paths);
    }

    [Fact]
    public void Cached_no_args_head_vs_index()
    {
        var git = GitWhereCommitsAre();

        var opts = new CommandLineOptions { Cached = true };

        var resolved = RefAndPathResolver.Resolve(git, "/repo", opts);

        Assert.Equal(GitRefSpec.HeadVsIndex, resolved.Refs);
    }

    [Fact]
    public void Single_commit_vs_working_tree()
    {
        var git = GitWhereCommitsAre("HEAD");

        var opts = new CommandLineOptions
        {
            PositionalArgs = new List<string> { "HEAD" },
        };

        var resolved = RefAndPathResolver.Resolve(git, "/repo", opts);

        Assert.Equal("HEAD", resolved.Refs.RefA);

        Assert.Equal(GitRefSpec.WorkingTree, resolved.Refs.RefB);
    }

    [Fact]
    public void Single_commit_cached_vs_index()
    {
        var git = GitWhereCommitsAre("HEAD");

        var opts = new CommandLineOptions
        {
            Cached = true,
            PositionalArgs = new List<string> { "HEAD" },
        };

        var resolved = RefAndPathResolver.Resolve(git, "/repo", opts);

        Assert.Equal("HEAD", resolved.Refs.RefA);

        Assert.Equal(GitRefSpec.Index, resolved.Refs.RefB);
    }

    [Fact]
    public void Two_commits()
    {
        var git = GitWhereCommitsAre("main", "feature");

        var opts = new CommandLineOptions
        {
            PositionalArgs = new List<string> { "main", "feature" },
        };

        var resolved = RefAndPathResolver.Resolve(git, "/repo", opts);

        Assert.Equal("main", resolved.Refs.RefA);

        Assert.Equal("feature", resolved.Refs.RefB);
    }

    [Fact]
    public void Cached_with_two_commits_rejected()
    {
        var git = GitWhereCommitsAre("a", "b");

        var opts = new CommandLineOptions
        {
            Cached = true,
            PositionalArgs = new List<string> { "a", "b" },
        };

        Assert.Throws<UsageException>(() => RefAndPathResolver.Resolve(git, "/repo", opts));
    }

    [Fact]
    public void Path_only_no_refs()
    {
        var git = GitWhereCommitsAre();

        var opts = new CommandLineOptions
        {
            PositionalArgs = new List<string> { "src/foo.cs" },
        };

        var resolved = RefAndPathResolver.Resolve(git, "/repo", opts);

        Assert.Equal(GitRefSpec.IndexVsWorkingTree, resolved.Refs);

        Assert.Equal(new[] { "src/foo.cs" }, resolved.Paths);
    }

    [Fact]
    public void Mixed_ref_then_path()
    {
        var git = GitWhereCommitsAre("HEAD");

        var opts = new CommandLineOptions
        {
            PositionalArgs = new List<string> { "HEAD", "src/foo.cs" },
        };

        var resolved = RefAndPathResolver.Resolve(git, "/repo", opts);

        Assert.Equal("HEAD", resolved.Refs.RefA);

        Assert.Equal(new[] { "src/foo.cs" }, resolved.Paths);
    }

    [Fact]
    public void Explicit_paths_after_double_dash_force_refs()
    {
        var git = GitWhereCommitsAre("HEAD");

        var opts = new CommandLineOptions
        {
            PositionalArgs = new List<string> { "HEAD" },
            ExplicitPaths = new List<string> { "src/foo.cs" },
        };

        var resolved = RefAndPathResolver.Resolve(git, "/repo", opts);

        Assert.Equal("HEAD", resolved.Refs.RefA);

        Assert.Equal(new[] { "src/foo.cs" }, resolved.Paths);
    }

    [Fact]
    public void Explicit_path_with_unknown_ref_throws()
    {
        var git = GitWhereCommitsAre();

        var opts = new CommandLineOptions
        {
            PositionalArgs = new List<string> { "not-a-ref" },
            ExplicitPaths = new List<string> { "src/foo.cs" },
        };

        Assert.Throws<UsageException>(() => RefAndPathResolver.Resolve(git, "/repo", opts));
    }
}
