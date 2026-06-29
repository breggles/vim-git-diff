using Xunit;

using VimGitDiff.Vim;

namespace VimGitDiff.Tests.Vim;

public class VimLauncherTests
{
    [Theory]
    [InlineData("gvim")]
    [InlineData("gvim.exe")]
    [InlineData("GVIM.EXE")]
    [InlineData("C:\\Program Files\\Vim\\gvim.exe")]
    [InlineData("/usr/bin/gvim")]
    public void Gui_executables_are_detected(string exe)
    {
        Assert.True(VimLauncher.IsGui(exe));
    }

    [Theory]
    [InlineData("vim")]
    [InlineData("vim.exe")]
    [InlineData("nvim")]
    [InlineData("/usr/bin/vim")]
    [InlineData("C:\\tools\\vim.exe")]
    public void Terminal_executables_are_not_gui(string exe)
    {
        Assert.False(VimLauncher.IsGui(exe));
    }
}
