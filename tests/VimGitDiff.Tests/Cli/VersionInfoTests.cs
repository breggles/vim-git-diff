using Xunit;

using VimGitDiff.Cli;

namespace VimGitDiff.Tests.Cli;

public class VersionInfoTests
{
    [Fact]
    public void Plain_version_is_returned_unchanged()
    {
        Assert.Equal("0.1.2", VersionInfo.Format("0.1.2"));
    }

    [Fact]
    public void Git_sha_suffix_is_stripped()
    {
        Assert.Equal("0.1.2", VersionInfo.Format("0.1.2+abc1234"));
    }

    [Fact]
    public void Null_is_unknown()
    {
        Assert.Equal("unknown", VersionInfo.Format(null));
    }

    [Fact]
    public void Empty_is_unknown()
    {
        Assert.Equal("unknown", VersionInfo.Format(""));
    }
}
