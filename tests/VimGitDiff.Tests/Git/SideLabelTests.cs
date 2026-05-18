using Xunit;

using VimGitDiff.Git;

namespace VimGitDiff.Tests.Git;

public class SideLabelTests
{
    [Fact]
    public void Present_side()
    {
        Assert.Equal("foo.cs@HEAD", SideLabel.Build("foo.cs", "foo.cs", "HEAD"));
    }

    [Fact]
    public void Missing_side_uses_input_name_and_marker()
    {
        Assert.Equal("foo.cs@HEAD (missing)", SideLabel.Build("foo.cs", null, "HEAD"));
    }

    [Fact]
    public void Renamed_resolves_to_resolved_path()
    {
        Assert.Equal("new.cs@HEAD", SideLabel.Build("old.cs", "new.cs", "HEAD"));
    }

    [Fact]
    public void Working_tree_label()
    {
        Assert.Equal("foo.cs@working tree", SideLabel.Build("foo.cs", "foo.cs", ""));
    }

    [Fact]
    public void Index_label()
    {
        Assert.Equal("foo.cs@index", SideLabel.Build("foo.cs", "foo.cs", ":0:"));
    }
}
