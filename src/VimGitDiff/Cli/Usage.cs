namespace VimGitDiff.Cli;

public static class Usage
{
    public const string Text = @"vim-git-diff - show git diffs in Vim

Usage:
    vim-git-diff [<options>] [<commit>] [--] [<path>...]
    vim-git-diff [<options>] <commit> <commit> [--] [<path>...]
    vim-git-diff [<options>] --cached [<commit>] [--] [<path>...]

Options:
    --cached, --staged   Diff against the index instead of the working tree.
    --vim <path>         Vim executable to launch (default: gvim, then vim).
                         Override via VIM_GIT_DIFF_VIM env var.
    -C <dir>             Run as if started in <dir>.
    --max-files <N>      Cap number of tabs (default 50).
    --all                Open all changed files (no cap).
    -h, --help           Show this help.
    --version            Show version.
";
}
