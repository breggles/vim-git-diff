namespace VimGitDiff.Git;

public sealed record GitRefSpec(string RefA, string RefB)
{
    public const string WorkingTree = "";

    public const string Index = ":0:";

    public static GitRefSpec IndexVsWorkingTree => new(Index, WorkingTree);

    public static GitRefSpec HeadVsIndex => new("HEAD", Index);

    public static GitRefSpec HeadVsWorkingTree => new("HEAD", WorkingTree);

    public bool UsesCached => RefA == Index || RefB == Index;
}
