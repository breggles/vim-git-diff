using System.Linq;

using Xunit;

using VimGitDiff.Git;
using VimGitDiff.Io;
using VimGitDiff.Tests.Fakes;

namespace VimGitDiff.Tests.Git;

public class GitRefClassifierTests
{
    [Fact]
    public void Dash_prefix_is_not_commitish()
    {
        var git = new FakeGitRunner();

        Assert.False(GitRefClassifier.IsCommitish(git, "/repo", "-foo"));

        Assert.Empty(git.Invocations);
    }

    [Fact]
    public void Commitish_when_rev_parse_exits_zero()
    {
        var git = new FakeGitRunner
        {
            Responder = (_, _) => new ProcessResult(0, "abc\n", ""),
        };

        Assert.True(GitRefClassifier.IsCommitish(git, "/repo", "HEAD"));

        Assert.Single(git.Invocations);

        var args = git.Invocations[0].Arguments;

        Assert.Equal("rev-parse", args[0]);

        Assert.Contains("HEAD^{commit}", args);
    }

    [Fact]
    public void Not_commitish_when_rev_parse_fails()
    {
        var git = new FakeGitRunner
        {
            Responder = (_, _) => new ProcessResult(128, "", "unknown"),
        };

        Assert.False(GitRefClassifier.IsCommitish(git, "/repo", "src/foo.cs"));
    }
}
