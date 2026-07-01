using System;
using System.Collections.Generic;
using System.IO;

using VimGitDiff.Errors;
using VimGitDiff.Git;
using VimGitDiff.Io;
using VimGitDiff.Vim;

namespace VimGitDiff.Cli;

public sealed class Driver
{
    private readonly TextWriter _stderr;

    private readonly IGitRunner _git;

    public Driver(TextWriter stderr, IGitRunner git)
    {
        _stderr = stderr;

        _git = git;
    }

    public ExitCode Run(CommandLineOptions options)
    {
        var startDir = options.ChangeDirectory ?? Directory.GetCurrentDirectory();

        if (!Directory.Exists(startDir))
        {
            _stderr.WriteLine($"vim-git-diff: directory does not exist: {startDir}");

            return ExitCode.Usage;
        }

        var repo = GitRepository.Discover(_git, startDir);

        var inputs = RefAndPathResolver.Resolve(_git, repo.Root, options);

        var changedFiles = ChangedFileEnumerator.Enumerate(_git, repo.Root, inputs.Refs, inputs.Paths);

        var binary = BinaryFilter.FindBinaryPaths(_git, repo.Root, inputs.Refs, changedFiles);

        var filtered = new List<ChangedFile>();

        foreach (var f in changedFiles)
        {
            var key = f.PathB ?? f.PathA ?? "";

            if (binary.Contains(key))
            {
                _stderr.WriteLine($"skipping binary: {f.DisplayPath}");

                continue;
            }

            filtered.Add(f);
        }

        if (filtered.Count == 0)
        {
            _stderr.WriteLine("no changes");

            return ExitCode.Ok;
        }

        var cap = options.All ? int.MaxValue : options.MaxFiles ?? 50;

        if (filtered.Count > cap)
        {
            _stderr.WriteLine($"truncated to {cap} of {filtered.Count} files (use --all to override)");

            filtered = filtered.GetRange(0, cap);
        }

        var extracted = new ContentExtractor(_git).Extract(repo.Root, inputs.Refs, filtered);

        var plan = new DiffPlan(repo.Root, inputs.Refs, filtered, extracted.Paths);

        var scriptText = new VimScriptBuilder().Build(plan);

        var script = TempScriptFile.Create(scriptText);

        var vimExe = VimDiscovery.Resolve(options.VimExecutable, Environment.GetEnvironmentVariable);

        var launcher = new VimLauncher();

        LaunchResult result;

        try
        {
            result = launcher.Launch(vimExe, script.Path);
        }
        catch
        {
            script.Dispose();

            DisposeAll(extracted.Files);

            throw;
        }

        if (!result.Detached)
        {
            script.Dispose();

            DisposeAll(extracted.Files);
        }

        return result.ExitCode == 0 ? ExitCode.Ok : ExitCode.Other;
    }

    private static void DisposeAll(IReadOnlyList<TempContentFile> files)
    {
        foreach (var file in files)
        {
            file.Dispose();
        }
    }
}
