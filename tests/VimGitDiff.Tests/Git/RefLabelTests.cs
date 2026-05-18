using Xunit;

using VimGitDiff.Git;

namespace VimGitDiff.Tests.Git;

public class RefLabelTests
{
    [Fact]
    public void Working_tree_is_empty_string()
    {
        Assert.Equal("working tree", RefLabel.For(""));
    }

    [Fact]
    public void Index_is_colon_zero_colon()
    {
        Assert.Equal("index", RefLabel.For(":0:"));
    }

    [Fact]
    public void Other_ref_passthrough()
    {
        Assert.Equal("HEAD", RefLabel.For("HEAD"));

        Assert.Equal("main", RefLabel.For("main"));
    }
}
