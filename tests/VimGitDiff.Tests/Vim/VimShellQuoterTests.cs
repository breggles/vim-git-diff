using Xunit;

using VimGitDiff.Vim;

namespace VimGitDiff.Tests.Vim;

public class VimShellQuoterTests
{
    [Fact]
    public void Cmd_wraps_in_double_quotes()
    {
        Assert.Equal("\"foo\"", VimShellQuoter.QuoteForCmd("foo"));
    }

    [Fact]
    public void Cmd_escapes_inner_double_quote()
    {
        Assert.Equal("\"a\\\"b\"", VimShellQuoter.QuoteForCmd("a\"b"));
    }

    [Fact]
    public void Posix_wraps_in_single_quotes()
    {
        Assert.Equal("'foo'", VimShellQuoter.QuoteForPosix("foo"));
    }

    [Fact]
    public void Posix_escapes_inner_single_quote()
    {
        Assert.Equal("'a'\\''b'", VimShellQuoter.QuoteForPosix("a'b"));
    }
}
