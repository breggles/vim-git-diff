using System.Collections.Generic;

using Xunit;

using VimGitDiff.Git;
using VimGitDiff.Vim;

namespace VimGitDiff.Tests.Vim;

public class VimScriptBuilderTests
{
    private static string BuildPosix(DiffPlan plan)
    {
        return new VimScriptBuilder(quoteForCmd: false).Build(plan);
    }

    [Fact]
    public void Header_contains_diffopt_and_cleanup_autocmd()
    {
        var plan = new DiffPlan("/repo", GitRefSpec.HeadVsWorkingTree, new List<ChangedFile>());

        var script = BuildPosix(plan);

        Assert.Contains("set diffopt+=algorithm:histogram", script);

        Assert.Contains("set diffopt+=linematch:60", script);

        Assert.Contains("autocmd VimLeave * silent! call delete(expand('<sfile>'))", script);
    }

    [Fact]
    public void Modified_file_HEAD_vs_working_tree()
    {
        var plan = new DiffPlan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Modified, "foo.cs", "foo.cs") });

        var script = BuildPosix(plan);

        Assert.Contains("tabnew", script);

        Assert.Contains("silent 0read !git -C '/repo' show 'HEAD:foo.cs'", script);

        Assert.Contains("foo.cs@HEAD", script);

        Assert.Contains("foo.cs@working\\ tree", script);

        Assert.Contains("diffthis", script);

        Assert.Contains("vert new", script);
    }

    [Fact]
    public void Added_file_left_side_is_missing()
    {
        var plan = new DiffPlan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Added, null, "new.cs") });

        var script = BuildPosix(plan);

        Assert.Contains("new.cs@HEAD\\ \\(missing\\)", script);

        Assert.DoesNotContain("git -C '/repo' show 'HEAD:new.cs'", script);
    }

    [Fact]
    public void Deleted_file_right_side_is_missing()
    {
        var plan = new DiffPlan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Deleted, "gone.cs", null) });

        var script = BuildPosix(plan);

        Assert.Contains("gone.cs@HEAD", script);

        Assert.Contains("gone.cs@working\\ tree\\ \\(missing\\)", script);
    }

    [Fact]
    public void Index_ref_uses_colon_path_spec()
    {
        var plan = new DiffPlan(
            "/repo",
            GitRefSpec.HeadVsIndex,
            new List<ChangedFile> { new(ChangeStatus.Modified, "foo.cs", "foo.cs") });

        var script = BuildPosix(plan);

        Assert.Contains("git -C '/repo' show ':foo.cs'", script);

        Assert.Contains("foo.cs@index", script);
    }

    [Fact]
    public void Working_tree_side_uses_local_read_no_shellout()
    {
        var plan = new DiffPlan(
            "/repo",
            GitRefSpec.IndexVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Modified, "foo.cs", "foo.cs") });

        var script = BuildPosix(plan);

        Assert.Matches(@"silent 0read (?:\\\\|/)repo(?:\\\\|/)foo\.cs", script);

        Assert.Contains("git -C '/repo' show ':foo.cs'", script);
    }

    [Fact]
    public void Two_files_open_two_tabs()
    {
        var plan = new DiffPlan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile>
            {
                new(ChangeStatus.Modified, "a.cs", "a.cs"),
                new(ChangeStatus.Modified, "b.cs", "b.cs"),
            });

        var script = BuildPosix(plan);

        var firstTab = script.IndexOf("tabnew", System.StringComparison.Ordinal);

        var secondTab = script.IndexOf("tabnew", firstTab + 1, System.StringComparison.Ordinal);

        Assert.True(firstTab >= 0);

        Assert.True(secondTab > firstTab);
    }

    [Fact]
    public void Rename_uses_different_paths_each_side()
    {
        var plan = new DiffPlan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Renamed, "old.cs", "new.cs") });

        var script = BuildPosix(plan);

        Assert.Contains("git -C '/repo' show 'HEAD:old.cs'", script);

        Assert.Contains("new.cs@working\\ tree", script);
    }
}
