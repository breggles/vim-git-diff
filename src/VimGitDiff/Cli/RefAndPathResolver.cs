using System.Collections.Generic;

using VimGitDiff.Errors;
using VimGitDiff.Git;

namespace VimGitDiff.Cli;

public sealed record ResolvedInputs(GitRefSpec Refs, IReadOnlyList<string> Paths);

public static class RefAndPathResolver
{
    public static ResolvedInputs Resolve(IGitRunner git, string repoRoot, CommandLineOptions options)
    {
        var positional = options.PositionalArgs;

        var refs = new List<string>();

        var paths = new List<string>();

        if (options.ExplicitPaths != null)
        {
            paths.AddRange(options.ExplicitPaths);

            foreach (var token in positional)
            {
                if (refs.Count >= 2)
                {
                    throw new UsageException("too many revisions");
                }

                if (!GitRefClassifier.IsCommitish(git, repoRoot, token))
                {
                    throw new UsageException($"unknown revision before --: {token}");
                }

                refs.Add(token);
            }
        }
        else
        {
            foreach (var token in positional)
            {
                if (refs.Count < 2 && GitRefClassifier.IsCommitish(git, repoRoot, token))
                {
                    refs.Add(token);
                }
                else
                {
                    paths.Add(token);
                }
            }
        }

        if (refs.Count > 2)
        {
            throw new UsageException("too many revisions");
        }

        var spec = BuildRefSpec(refs, options.Cached);

        return new ResolvedInputs(spec, paths);
    }

    private static GitRefSpec BuildRefSpec(IReadOnlyList<string> refs, bool cached)
    {
        if (cached && refs.Count >= 2)
        {
            throw new UsageException("--cached cannot be combined with two revisions");
        }

        if (refs.Count == 0)
        {
            return cached ? GitRefSpec.HeadVsIndex : GitRefSpec.IndexVsWorkingTree;
        }

        if (refs.Count == 1)
        {
            var a = refs[0];

            var b = cached ? GitRefSpec.Index : GitRefSpec.WorkingTree;

            return new GitRefSpec(a, b);
        }

        return new GitRefSpec(refs[0], refs[1]);
    }
}
