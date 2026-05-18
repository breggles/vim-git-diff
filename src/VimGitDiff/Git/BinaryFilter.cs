using System.Collections.Generic;

namespace VimGitDiff.Git;

public static class BinaryFilter
{
    public static HashSet<string> FindBinaryPaths(
        IGitRunner git,
        string repoRoot,
        GitRefSpec refs,
        IReadOnlyList<ChangedFile> changedFiles)
    {
        var binary = new HashSet<string>();

        if (changedFiles.Count == 0)
        {
            return binary;
        }

        var args = new List<string>
        {
            "diff",
            "-M",
            "--numstat",
            "-z",
        };

        AppendRefArgs(args, refs);

        args.Add("--");

        foreach (var f in changedFiles)
        {
            if (f.PathB != null)
            {
                args.Add(f.PathB);
            }
            else if (f.PathA != null)
            {
                args.Add(f.PathA);
            }
        }

        var result = git.Run(args, repoRoot);

        if (result.ExitCode != 0)
        {
            return binary;
        }

        foreach (var path in ParseBinaryPaths(result.StdOut))
        {
            binary.Add(path);
        }

        return binary;
    }

    private static void AppendRefArgs(List<string> args, GitRefSpec refs)
    {
        if (refs.RefA == GitRefSpec.Index && refs.RefB == GitRefSpec.WorkingTree)
        {
            return;
        }

        if (refs.RefA == "HEAD" && refs.RefB == GitRefSpec.Index)
        {
            args.Add("--cached");

            return;
        }

        if (refs.RefB == GitRefSpec.Index)
        {
            args.Add("--cached");

            args.Add(refs.RefA);

            return;
        }

        if (refs.RefB == GitRefSpec.WorkingTree)
        {
            args.Add(refs.RefA);

            return;
        }

        args.Add(refs.RefA);

        args.Add(refs.RefB);
    }

    public static IEnumerable<string> ParseBinaryPaths(string numstatZ)
    {
        var tokens = numstatZ.Split('\0');

        var i = 0;

        while (i < tokens.Length)
        {
            var line = tokens[i];

            if (string.IsNullOrEmpty(line))
            {
                i++;

                continue;
            }

            var parts = line.Split('\t');

            if (parts.Length < 3)
            {
                i++;

                continue;
            }

            var added = parts[0];

            var deleted = parts[1];

            var pathField = parts[2];

            var isBinary = added == "-" && deleted == "-";

            if (pathField.Length == 0)
            {
                if (i + 2 >= tokens.Length)
                {
                    break;
                }

                var newName = tokens[i + 2];

                if (isBinary)
                {
                    yield return newName;
                }

                i += 3;

                continue;
            }

            if (isBinary)
            {
                yield return pathField;
            }

            i++;
        }
    }
}
