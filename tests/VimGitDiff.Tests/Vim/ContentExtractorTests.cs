using System.Collections.Generic;
using System.IO;

using Xunit;

using VimGitDiff.Git;
using VimGitDiff.Io;
using VimGitDiff.Tests.Fakes;
using VimGitDiff.Vim;

namespace VimGitDiff.Tests.Vim;

public class ContentExtractorTests
{
    [Fact]
    public void Named_ref_side_runs_git_show_with_ref_colon_path()
    {
        var git = new FakeGitRunner { Responder = (_, _) => new ProcessResult(0, "old\n", "") };

        var extracted = new ContentExtractor(git).Extract(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Modified, "foo.cs", "foo.cs") });

        var inv = Assert.Single(git.Invocations);

        Assert.Equal(new[] { "show", "HEAD:foo.cs" }, inv.Arguments);

        Assert.Equal("/repo", inv.WorkingDirectory);

        DisposeAll(extracted);
    }

    [Fact]
    public void Index_ref_side_runs_git_show_with_colon_path()
    {
        var git = new FakeGitRunner { Responder = (_, _) => new ProcessResult(0, "staged\n", "") };

        var extracted = new ContentExtractor(git).Extract(
            "/repo",
            GitRefSpec.HeadVsIndex,
            new List<ChangedFile> { new(ChangeStatus.Modified, "foo.cs", "foo.cs") });

        Assert.Equal(new[] { "show", "HEAD:foo.cs" }, git.Invocations[0].Arguments);

        Assert.Equal(new[] { "show", ":foo.cs" }, git.Invocations[1].Arguments);

        DisposeAll(extracted);
    }

    [Fact]
    public void Working_tree_side_is_not_extracted()
    {
        var git = new FakeGitRunner { Responder = (_, _) => new ProcessResult(0, "x\n", "") };

        var extracted = new ContentExtractor(git).Extract(
            "/repo",
            GitRefSpec.IndexVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Modified, "foo.cs", "foo.cs") });

        var inv = Assert.Single(git.Invocations);

        Assert.Equal(new[] { "show", ":foo.cs" }, inv.Arguments);

        Assert.True(extracted.Paths.ContainsKey((0, true)));

        Assert.False(extracted.Paths.ContainsKey((0, false)));

        DisposeAll(extracted);
    }

    [Fact]
    public void Missing_side_is_not_extracted()
    {
        var git = new FakeGitRunner { Responder = (_, _) => new ProcessResult(0, "added\n", "") };

        var extracted = new ContentExtractor(git).Extract(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Added, null, "new.cs") });

        Assert.Empty(git.Invocations);

        Assert.Empty(extracted.Paths);

        DisposeAll(extracted);
    }

    [Fact]
    public void Extracted_content_is_written_to_temp_file()
    {
        var git = new FakeGitRunner { Responder = (_, _) => new ProcessResult(0, "hello\n", "") };

        var extracted = new ContentExtractor(git).Extract(
            "/repo",
            GitRefSpec.HeadVsWorkingTree,
            new List<ChangedFile> { new(ChangeStatus.Modified, "foo.cs", "foo.cs") });

        var path = extracted.Paths[(0, true)];

        Assert.Equal("hello\n", File.ReadAllText(path));

        DisposeAll(extracted);

        Assert.False(File.Exists(path));
    }

    private static void DisposeAll(ExtractedContent extracted)
    {
        foreach (var file in extracted.Files)
        {
            file.Dispose();
        }
    }
}
