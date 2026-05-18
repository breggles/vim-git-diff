using Xunit;

using VimGitDiff.Git;

namespace VimGitDiff.Tests.Git;

public class ChangedFileEnumeratorTests
{
    [Fact]
    public void Modified()
    {
        var entries = ChangedFileEnumerator.Parse("M\0foo.cs\0");

        Assert.Single(entries);

        Assert.Equal(ChangeStatus.Modified, entries[0].Status);

        Assert.Equal("foo.cs", entries[0].PathA);

        Assert.Equal("foo.cs", entries[0].PathB);
    }

    [Fact]
    public void Added()
    {
        var entries = ChangedFileEnumerator.Parse("A\0new.cs\0");

        Assert.Single(entries);

        Assert.Equal(ChangeStatus.Added, entries[0].Status);

        Assert.Null(entries[0].PathA);

        Assert.Equal("new.cs", entries[0].PathB);
    }

    [Fact]
    public void Deleted()
    {
        var entries = ChangedFileEnumerator.Parse("D\0gone.cs\0");

        Assert.Single(entries);

        Assert.Equal(ChangeStatus.Deleted, entries[0].Status);

        Assert.Equal("gone.cs", entries[0].PathA);

        Assert.Null(entries[0].PathB);
    }

    [Fact]
    public void Renamed()
    {
        var entries = ChangedFileEnumerator.Parse("R100\0old.cs\0new.cs\0");

        Assert.Single(entries);

        Assert.Equal(ChangeStatus.Renamed, entries[0].Status);

        Assert.Equal("old.cs", entries[0].PathA);

        Assert.Equal("new.cs", entries[0].PathB);
    }

    [Fact]
    public void Copied()
    {
        var entries = ChangedFileEnumerator.Parse("C75\0src.cs\0copy.cs\0");

        Assert.Single(entries);

        Assert.Equal(ChangeStatus.Copied, entries[0].Status);
    }

    [Fact]
    public void Type_changed()
    {
        var entries = ChangedFileEnumerator.Parse("T\0link\0");

        Assert.Single(entries);

        Assert.Equal(ChangeStatus.TypeChanged, entries[0].Status);
    }

    [Fact]
    public void Multiple_entries()
    {
        var entries = ChangedFileEnumerator.Parse("M\0a\0A\0b\0R90\0c\0d\0D\0e\0");

        Assert.Equal(4, entries.Count);

        Assert.Equal(ChangeStatus.Modified, entries[0].Status);

        Assert.Equal(ChangeStatus.Added, entries[1].Status);

        Assert.Equal(ChangeStatus.Renamed, entries[2].Status);

        Assert.Equal(ChangeStatus.Deleted, entries[3].Status);
    }

    [Fact]
    public void Empty_input()
    {
        var entries = ChangedFileEnumerator.Parse("");

        Assert.Empty(entries);
    }
}
