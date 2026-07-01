using System;

using VimGitDiff.Cli;
using VimGitDiff.Errors;
using VimGitDiff.Git;
using VimGitDiff.Io;

namespace VimGitDiff;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            var options = ArgumentParser.Parse(args);

            if (options.Help)
            {
                Console.Out.Write(Usage.Text);

                return (int)ExitCode.Ok;
            }

            if (options.Version)
            {
                Console.Out.WriteLine("vim-git-diff " + VersionInfo.Current());

                return (int)ExitCode.Ok;
            }

            var processRunner = new ProcessRunner();

            var gitRunner = new GitRunner(processRunner);

            var driver = new Driver(Console.Error, gitRunner);

            return (int)driver.Run(options);
        }
        catch (UsageException ex)
        {
            Console.Error.WriteLine($"vim-git-diff: {ex.Message}");

            Console.Error.WriteLine();

            Console.Error.Write(Usage.Text);

            return (int)ExitCode.Usage;
        }
        catch (NotAGitRepoException ex)
        {
            Console.Error.WriteLine($"vim-git-diff: {ex.Message}");

            return (int)ExitCode.NotAGitRepo;
        }
        catch (BadRefException ex)
        {
            Console.Error.WriteLine($"vim-git-diff: {ex.Message}");

            return (int)ExitCode.BadRef;
        }
        catch (GitNotFoundException ex)
        {
            Console.Error.WriteLine($"vim-git-diff: {ex.Message}");

            return (int)ExitCode.GitMissing;
        }
        catch (VimNotFoundException ex)
        {
            Console.Error.WriteLine($"vim-git-diff: {ex.Message}");

            return (int)ExitCode.VimMissing;
        }
        catch (NoChangesException ex)
        {
            Console.Error.WriteLine($"vim-git-diff: {ex.Message}");

            return (int)ExitCode.Ok;
        }
        catch (VimGitDiffException ex)
        {
            Console.Error.WriteLine($"vim-git-diff: {ex.Message}");

            return (int)ExitCode.Other;
        }
    }
}
