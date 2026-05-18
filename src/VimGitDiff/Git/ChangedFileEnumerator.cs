using System.Collections.Generic;

using VimGitDiff.Errors;

namespace VimGitDiff.Git;

public static class ChangedFileEnumerator
{
    public static IReadOnlyList<ChangedFile> Enumerate(
        IGitRunner git,
        string repoRoot,
        GitRefSpec refs,
        IReadOnlyList<string> paths)
    {
        var args = new List<string>
        {
            "diff",
            "-M",
            "--name-status",
            "-z",
        };

        AppendRefArgs(args, refs);

        if (paths.Count > 0)
        {
            args.Add("--");

            foreach (var p in paths)
            {
                args.Add(p);
            }
        }

        var result = git.Run(args, repoRoot);

        if (result.ExitCode != 0)
        {
            throw new BadRefException($"git diff failed: {result.StdErr.Trim()}");
        }

        return Parse(result.StdOut);
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

    public static IReadOnlyList<ChangedFile> Parse(string nameStatusZ)
    {
        var entries = new List<ChangedFile>();

        var tokens = nameStatusZ.Split('\0');

        var i = 0;

        while (i < tokens.Length)
        {
            var status = tokens[i];

            if (string.IsNullOrEmpty(status))
            {
                i++;

                continue;
            }

            var code = status[0];

            if (code == 'R' || code == 'C')
            {
                if (i + 2 >= tokens.Length)
                {
                    break;
                }

                var oldName = tokens[i + 1];

                var newName = tokens[i + 2];

                var kind = code == 'R' ? ChangeStatus.Renamed : ChangeStatus.Copied;

                entries.Add(new ChangedFile(kind, oldName, newName));

                i += 3;

                continue;
            }

            if (i + 1 >= tokens.Length)
            {
                break;
            }

            var path = tokens[i + 1];

            switch (code)
            {
                case 'M':
                    entries.Add(new ChangedFile(ChangeStatus.Modified, path, path));

                    break;

                case 'A':
                    entries.Add(new ChangedFile(ChangeStatus.Added, null, path));

                    break;

                case 'D':
                    entries.Add(new ChangedFile(ChangeStatus.Deleted, path, null));

                    break;

                case 'T':
                    entries.Add(new ChangedFile(ChangeStatus.TypeChanged, path, path));

                    break;

                default:
                    entries.Add(new ChangedFile(ChangeStatus.Modified, path, path));

                    break;
            }

            i += 2;
        }

        return entries;
    }
}
