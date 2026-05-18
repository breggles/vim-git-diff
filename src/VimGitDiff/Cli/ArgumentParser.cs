using System.Collections.Generic;

using VimGitDiff.Errors;

namespace VimGitDiff.Cli;

public static class ArgumentParser
{
    public static CommandLineOptions Parse(IReadOnlyList<string> args)
    {
        var cached = false;

        string? vim = null;

        string? cd = null;

        int? maxFiles = null;

        var all = false;

        var help = false;

        var version = false;

        var positional = new List<string>();

        List<string>? explicitPaths = null;

        var i = 0;

        var sawDoubleDash = false;

        while (i < args.Count)
        {
            var arg = args[i];

            if (sawDoubleDash)
            {
                explicitPaths ??= new List<string>();

                explicitPaths.Add(arg);

                i++;

                continue;
            }

            if (arg == "--")
            {
                sawDoubleDash = true;

                explicitPaths ??= new List<string>();

                i++;

                continue;
            }

            if (arg == "-h" || arg == "--help")
            {
                help = true;

                i++;

                continue;
            }

            if (arg == "--version")
            {
                version = true;

                i++;

                continue;
            }

            if (arg == "--cached" || arg == "--staged")
            {
                cached = true;

                i++;

                continue;
            }

            if (arg == "--all")
            {
                all = true;

                i++;

                continue;
            }

            if (arg == "--vim")
            {
                if (i + 1 >= args.Count)
                {
                    throw new UsageException("--vim requires a value");
                }

                vim = args[i + 1];

                i += 2;

                continue;
            }

            if (arg.StartsWith("--vim="))
            {
                vim = arg.Substring("--vim=".Length);

                i++;

                continue;
            }

            if (arg == "-C")
            {
                if (i + 1 >= args.Count)
                {
                    throw new UsageException("-C requires a value");
                }

                cd = args[i + 1];

                i += 2;

                continue;
            }

            if (arg == "--max-files")
            {
                if (i + 1 >= args.Count)
                {
                    throw new UsageException("--max-files requires a value");
                }

                if (!int.TryParse(args[i + 1], out var parsed) || parsed <= 0)
                {
                    throw new UsageException($"--max-files requires a positive integer, got: {args[i + 1]}");
                }

                maxFiles = parsed;

                i += 2;

                continue;
            }

            if (arg.StartsWith("--max-files="))
            {
                var rawValue = arg.Substring("--max-files=".Length);

                if (!int.TryParse(rawValue, out var parsed) || parsed <= 0)
                {
                    throw new UsageException($"--max-files requires a positive integer, got: {rawValue}");
                }

                maxFiles = parsed;

                i++;

                continue;
            }

            if (arg.StartsWith('-') && arg.Length > 1)
            {
                throw new UsageException($"unknown option: {arg}");
            }

            positional.Add(arg);

            i++;
        }

        return new CommandLineOptions
        {
            Cached = cached,
            VimExecutable = vim,
            ChangeDirectory = cd,
            MaxFiles = maxFiles,
            All = all,
            Help = help,
            Version = version,
            PositionalArgs = positional,
            ExplicitPaths = explicitPaths,
        };
    }
}
