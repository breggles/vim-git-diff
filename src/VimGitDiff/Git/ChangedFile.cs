namespace VimGitDiff.Git;

public sealed record ChangedFile(ChangeStatus Status, string? PathA, string? PathB)
{
    public string DisplayPath => PathB ?? PathA ?? "";
}
