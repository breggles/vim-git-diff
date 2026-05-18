using Xunit;

using VimGitDiff.Vim;

namespace VimGitDiff.Tests.Vim;

public class VimFilenameEscaperTests
{
    [Fact]
    public void Plain_chars_unchanged()
    {
        Assert.Equal("foo.cs", VimFilenameEscaper.Escape("foo.cs"));
    }

    [Fact]
    public void Space_is_escaped()
    {
        Assert.Equal("a\\ b", VimFilenameEscaper.Escape("a b"));
    }

    [Fact]
    public void Percent_hash_pipe_escaped()
    {
        Assert.Equal("\\%\\#\\|", VimFilenameEscaper.Escape("%#|"));
    }

    [Fact]
    public void Backslash_quote_apostrophe_escaped()
    {
        Assert.Equal("\\\\\\\"\\'", VimFilenameEscaper.Escape("\\\"'"));
    }

    [Fact]
    public void Brackets_braces_parens_escaped()
    {
        Assert.Equal("\\[\\]\\{\\}\\(\\)", VimFilenameEscaper.Escape("[]{}()"));
    }

    [Fact]
    public void Newline_and_tab_escaped()
    {
        Assert.Equal("\\\n\\\t", VimFilenameEscaper.Escape("\n\t"));
    }

    [Fact]
    public void Control_char_below_0x20_escaped()
    {
        Assert.Equal("\\\x01", VimFilenameEscaper.Escape("\x01"));
    }
}
