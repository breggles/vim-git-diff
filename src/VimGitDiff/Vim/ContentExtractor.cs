using System.Collections.Generic;
using System.Text;

using VimGitDiff.Git;
using VimGitDiff.Io;

namespace VimGitDiff.Vim;

public sealed class ExtractedContent
{
    public IReadOnlyDictionary<(int FileIndex, bool IsLeft), string> Paths { get; }

    public IReadOnlyList<TempContentFile> Files { get; }

    public ExtractedContent(
        IReadOnlyDictionary<(int FileIndex, bool IsLeft), string> paths,
        IReadOnlyList<TempContentFile> files)
    {
        Paths = paths;

        Files = files;
    }
}

public sealed class ContentExtractor
{
    private readonly IGitRunner _git;

    public ContentExtractor(IGitRunner git)
    {
        _git = git;
    }

    public ExtractedContent Extract(string repoRoot, GitRefSpec refs, IReadOnlyList<ChangedFile> files)
    {
        var paths = new Dictionary<(int, bool), string>();

        var temps = new List<TempContentFile>();

        for (var i = 0; i < files.Count; i++)
        {
            var file = files[i];

            AddSide(repoRoot, refs.RefA, file.PathA, i, isLeft: true, paths, temps);

            AddSide(repoRoot, refs.RefB, file.PathB, i, isLeft: false, paths, temps);
        }

        return new ExtractedContent(paths, temps);
    }

    private void AddSide(
        string repoRoot,
        string @ref,
        string? path,
        int fileIndex,
        bool isLeft,
        Dictionary<(int, bool), string> paths,
        List<TempContentFile> temps)
    {
        if (path == null || @ref == GitRefSpec.WorkingTree)
        {
            return;
        }

        var spec = @ref == GitRefSpec.Index ? ":" + path : @ref + ":" + path;

        var result = _git.Run(new[] { "show", spec }, repoRoot);

        var bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetBytes(result.StdOut);

        var temp = TempContentFile.Create(bytes);

        temps.Add(temp);

        paths[(fileIndex, isLeft)] = temp.Path;
    }
}
