using System.Collections.Generic;

using Xunit;

using VimGitDiff.Git;
using VimGitDiff.Vim;

namespace VimGitDiff.Tests.Vim;

public class VimScriptBuilderTests
{
    private static string Build(DiffPlan plan)
    {
        return new VimScriptBuilder().Build(plan);
    }

    private static DiffPlan Plan(
        string root,
        GitRefSpec refs,
        IReadOnlyList<ChangedFile> files,
        IReadOnlyDictionary<(int, bool), string>? content = null)
    {
        return new DiffPlan(root, refs, files, content);
    }

    [Fact]
    public void Header_contains_diffopt_and_cleanup_autocmd()
    {
        var plan = Plan("/repo", GitRefSpec.HeadVsWorkingTree, new List<ChangedFile>());

        var script = Build(plan);

        Assert.Contains("set diffopt+=algorithm:histogram", script);

        Assert.Contains("set diffopt+=linematch:60", script);

        Assert.Contains("let s:vgd_script = expand('<sfile>:p')", script);

        Assert.Contains("let s:vgd_temps = [s:vgd_script]", script);

        Assert.Contains("autocmd VimLeave * silent! call map(s:vgd_temps, 'delete(v:val)')", script);
    }

    [Fact]
    public void Modified_file_HEAD_vs_working_tree_reads_extracted_content_no_shellout()
    {
        var content = new Dictionary<(int, bool), string> { [(0, true)] = "/tmp/vgd/left.content" };

        var plan = Plan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Modified, "foo.cs", "foo.cs") },
            content);

        var script = Build(plan);

        Assert.Contains("tabnew", script);

        Assert.Contains("silent 0read /tmp/vgd/left.content", script);

        Assert.DoesNotContain("git show", script);

        Assert.DoesNotContain("0read !", script);

        Assert.Contains("foo.cs@HEAD", script);

        Assert.Contains("foo.cs@working\\ tree", script);

        Assert.Contains("diffthis", script);

        Assert.Contains("vert new", script);
    }

    [Fact]
    public void Content_temp_files_are_added_to_cleanup_list()
    {
        var content = new Dictionary<(int, bool), string>
        {
            [(0, true)] = "/tmp/vgd/left.content",
        };

        var plan = Plan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Modified, "foo.cs", "foo.cs") },
            content);

        var script = Build(plan);

        Assert.Contains("let s:vgd_temps = [s:vgd_script, '/tmp/vgd/left.content']", script);
    }

    [Fact]
    public void Added_file_left_side_is_missing()
    {
        var content = new Dictionary<(int, bool), string> { [(0, false)] = "/tmp/vgd/right.content" };

        var plan = Plan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Added, null, "new.cs") },
            content);

        var script = Build(plan);

        Assert.Contains("new.cs@HEAD\\ \\(missing\\)", script);
    }

    [Fact]
    public void Deleted_file_right_side_is_missing()
    {
        var content = new Dictionary<(int, bool), string> { [(0, true)] = "/tmp/vgd/left.content" };

        var plan = Plan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Deleted, "gone.cs", null) },
            content);

        var script = Build(plan);

        Assert.Contains("gone.cs@HEAD", script);

        Assert.Contains("gone.cs@working\\ tree\\ \\(missing\\)", script);
    }

    [Fact]
    public void Working_tree_side_uses_local_read()
    {
        var content = new Dictionary<(int, bool), string> { [(0, true)] = "/tmp/vgd/left.content" };

        var plan = Plan(
            "/repo",
            GitRefSpec.IndexVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Modified, "foo.cs", "foo.cs") },
            content);

        var script = Build(plan);

        Assert.Matches(@"silent 0read (?:\\\\|/)repo(?:\\\\|/)foo\.cs", script);
    }

    [Fact]
    public void Two_files_open_two_tabs()
    {
        var content = new Dictionary<(int, bool), string>
        {
            [(0, true)] = "/tmp/vgd/a.content",
            [(1, true)] = "/tmp/vgd/b.content",
        };

        var plan = Plan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile>
            {
                new(ChangeStatus.Modified, "a.cs", "a.cs"),
                new(ChangeStatus.Modified, "b.cs", "b.cs"),
            },
            content);

        var script = Build(plan);

        var firstTab = script.IndexOf("tabnew", System.StringComparison.Ordinal);

        var secondTab = script.IndexOf("tabnew", firstTab + 1, System.StringComparison.Ordinal);

        Assert.True(firstTab >= 0);

        Assert.True(secondTab > firstTab);
    }

    [Fact]
    public void Rename_uses_different_content_each_side()
    {
        var content = new Dictionary<(int, bool), string> { [(0, true)] = "/tmp/vgd/old.content" };

        var plan = Plan(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Renamed, "old.cs", "new.cs") },
            content);

        var script = Build(plan);

        Assert.Contains("silent 0read /tmp/vgd/old.content", script);

        Assert.Contains("new.cs@working\\ tree", script);
    }
}
