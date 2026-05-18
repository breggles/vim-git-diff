using Xunit;

using VimGitDiff.Cli;
using VimGitDiff.Errors;

namespace VimGitDiff.Tests.Cli;

public class ArgumentParserTests
{
    [Fact]
    public void Empty_args_returns_defaults()
    {
        var opts = ArgumentParser.Parse(new string[0]);

        Assert.False(opts.Cached);

        Assert.False(opts.Help);

        Assert.False(opts.Version);

        Assert.Null(opts.MaxFiles);

        Assert.False(opts.All);

        Assert.Empty(opts.PositionalArgs);

        Assert.Null(opts.ExplicitPaths);
    }

    [Fact]
    public void Cached_flag()
    {
        var opts = ArgumentParser.Parse(new[] { "--cached" });

        Assert.True(opts.Cached);
    }

    [Fact]
    public void Staged_alias()
    {
        var opts = ArgumentParser.Parse(new[] { "--staged" });

        Assert.True(opts.Cached);
    }

    [Fact]
    public void Help_flag()
    {
        var opts = ArgumentParser.Parse(new[] { "--help" });

        Assert.True(opts.Help);
    }

    [Fact]
    public void Vim_explicit_separate()
    {
        var opts = ArgumentParser.Parse(new[] { "--vim", "C:\\bin\\gvim.exe" });

        Assert.Equal("C:\\bin\\gvim.exe", opts.VimExecutable);
    }

    [Fact]
    public void Vim_explicit_equals()
    {
        var opts = ArgumentParser.Parse(new[] { "--vim=gvim" });

        Assert.Equal("gvim", opts.VimExecutable);
    }

    [Fact]
    public void Vim_missing_value_throws()
    {
        Assert.Throws<UsageException>(() => ArgumentParser.Parse(new[] { "--vim" }));
    }

    [Fact]
    public void ChangeDirectory()
    {
        var opts = ArgumentParser.Parse(new[] { "-C", "some\\dir" });

        Assert.Equal("some\\dir", opts.ChangeDirectory);
    }

    [Fact]
    public void MaxFiles_positive()
    {
        var opts = ArgumentParser.Parse(new[] { "--max-files", "10" });

        Assert.Equal(10, opts.MaxFiles);
    }

    [Fact]
    public void MaxFiles_equals()
    {
        var opts = ArgumentParser.Parse(new[] { "--max-files=10" });

        Assert.Equal(10, opts.MaxFiles);
    }

    [Fact]
    public void MaxFiles_zero_throws()
    {
        Assert.Throws<UsageException>(() => ArgumentParser.Parse(new[] { "--max-files", "0" }));
    }

    [Fact]
    public void All_flag()
    {
        var opts = ArgumentParser.Parse(new[] { "--all" });

        Assert.True(opts.All);
    }

    [Fact]
    public void Unknown_option_throws()
    {
        Assert.Throws<UsageException>(() => ArgumentParser.Parse(new[] { "--bogus" }));
    }

    [Fact]
    public void Positionals_collected()
    {
        var opts = ArgumentParser.Parse(new[] { "HEAD", "main" });

        Assert.Equal(new[] { "HEAD", "main" }, opts.PositionalArgs);

        Assert.Null(opts.ExplicitPaths);
    }

    [Fact]
    public void Double_dash_splits_paths()
    {
        var opts = ArgumentParser.Parse(new[] { "HEAD", "--", "src/foo.cs", "bar.txt" });

        Assert.Equal(new[] { "HEAD" }, opts.PositionalArgs);

        Assert.NotNull(opts.ExplicitPaths);

        Assert.Equal(new[] { "src/foo.cs", "bar.txt" }, opts.ExplicitPaths);
    }

    [Fact]
    public void Double_dash_with_no_paths()
    {
        var opts = ArgumentParser.Parse(new[] { "HEAD", "--" });

        Assert.NotNull(opts.ExplicitPaths);

        Assert.Empty(opts.ExplicitPaths!);
    }

    [Fact]
    public void Path_after_double_dash_can_start_with_dash()
    {
        var opts = ArgumentParser.Parse(new[] { "--", "-weird-name.txt" });

        Assert.Equal(new[] { "-weird-name.txt" }, opts.ExplicitPaths);
    }
}
