using System.Collections.Generic;

using VimGitDiff.Git;

namespace VimGitDiff.Vim;

public sealed record DiffPlan(string RepoRoot, GitRefSpec Refs, IReadOnlyList<ChangedFile> Files);
