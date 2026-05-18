namespace VimGitDiff.Git;

public static class SideLabel
{
    public static string Build(string relPathInput, string? resolvedPath, string @ref)
    {
        var name = resolvedPath ?? relPathInput;

        var missingMarker = resolvedPath is null ? " (missing)" : "";

        return $"{name}@{RefLabel.For(@ref)}{missingMarker}";
    }
}
