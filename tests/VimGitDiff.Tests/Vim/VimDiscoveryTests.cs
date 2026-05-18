using System.Collections.Generic;

using Xunit;

using VimGitDiff.Errors;
using VimGitDiff.Vim;

namespace VimGitDiff.Tests.Vim;

public class VimDiscoveryTests
{
    [Fact]
    public void Explicit_path_wins()
    {
        var resolved = VimDiscovery.Resolve("C:\\bin\\my-vim.exe", _ => null);

        Assert.Equal("C:\\bin\\my-vim.exe", resolved);
    }

    [Fact]
    public void Env_var_used_when_no_explicit()
    {
        var env = new Dictionary<string, string?>
        {
            [VimDiscovery.EnvVar] = "/usr/local/bin/my-vim",
        };

        var resolved = VimDiscovery.Resolve(null, k => env.TryGetValue(k, out var v) ? v : null);

        Assert.Equal("/usr/local/bin/my-vim", resolved);
    }

    [Fact]
    public void Throws_when_nothing_found()
    {
        Assert.Throws<VimNotFoundException>(() =>
            VimDiscovery.Resolve(null, _ => null));
    }
}
