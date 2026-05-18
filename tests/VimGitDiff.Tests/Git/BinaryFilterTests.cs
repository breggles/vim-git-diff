using System.Collections.Generic;
using System.Linq;

using Xunit;

using VimGitDiff.Git;

namespace VimGitDiff.Tests.Git;

public class BinaryFilterTests
{
    [Fact]
    public void Text_files_not_binary()
    {
        var paths = BinaryFilter.ParseBinaryPaths("5\t3\tfoo.cs\0").ToList();

        Assert.Empty(paths);
    }

    [Fact]
    public void Dash_dash_marks_binary()
    {
        var paths = BinaryFilter.ParseBinaryPaths("-\t-\timage.png\0").ToList();

        Assert.Equal(new[] { "image.png" }, paths);
    }

    [Fact]
    public void Mixed_lines()
    {
        var input = "5\t3\tfoo.cs\0-\t-\timage.png\0";

        var paths = BinaryFilter.ParseBinaryPaths(input).ToList();

        Assert.Equal(new[] { "image.png" }, paths);
    }

    [Fact]
    public void Binary_rename_uses_new_name()
    {
        var input = "-\t-\t\0old.bin\0new.bin\0";

        var paths = BinaryFilter.ParseBinaryPaths(input).ToList();

        Assert.Equal(new[] { "new.bin" }, paths);
    }
}
