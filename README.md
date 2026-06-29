# vim-git-diff

A command-line tool that shows git diffs in Vim, the way `git diff` shows
them in your terminal. Each changed file opens in its own tab as a
side-by-side Vim diff. By default it launches gvim.

When it launches gvim (the GUI), control returns to your shell immediately;
the gvim window cleans up its own temp script when you quit. A terminal
`vim` stays attached to the console until you quit it.

## Why

`git diff` is great for skimming small changes, but for non-trivial diffs
you want Vim's syntax highlighting, navigation, and `:diffget` / `:diffput`.
This tool spawns a fresh Vim instance, asks git for the diff inputs from
inside Vim, and lays out one tab per file.

There is no LLM, no MCP server, no plugin to install in Vim. It's a
standalone executable that orchestrates `git` and `vim`.

## Requirements

- .NET 10 SDK (to build) or the .NET 10 runtime (to run a framework-dependent
  publish).
- `git` on `PATH`.
- A working `gvim` or `vim` on `PATH` (or pointed at via `--vim` /
  `VIM_GIT_DIFF_VIM`).

## Build

```sh
dotnet build -c Release
```

Or a self-contained single-file publish:

```sh
dotnet publish src/VimGitDiff/VimGitDiff.csproj \
    -c Release \
    -r win-x64 \
    --self-contained true \
    -p:PublishSingleFile=true
```

Drop the resulting `vim-git-diff(.exe)` somewhere on your `PATH`.

## Usage

Mirrors `git diff` argument grammar:

```
vim-git-diff [<options>] [<commit>] [--] [<path>...]
vim-git-diff [<options>] <commit> <commit> [--] [<path>...]
vim-git-diff [<options>] --cached [<commit>] [--] [<path>...]
```

Examples:

```sh
# index vs working tree (== `git diff`)
vim-git-diff

# HEAD vs index (== `git diff --cached`)
vim-git-diff --cached

# HEAD vs working tree
vim-git-diff HEAD

# main vs feature
vim-git-diff main feature

# restrict to certain paths
vim-git-diff HEAD -- src/foo.cs README.md
```

### Options

| Option               | Description                                                           |
| -------------------- | --------------------------------------------------------------------- |
| `--cached`, `--staged` | Diff against the index instead of the working tree.                  |
| `--vim <path>`       | Vim executable to launch. Override via `VIM_GIT_DIFF_VIM`.            |
| `-C <dir>`           | Run as if started in `<dir>`.                                          |
| `--max-files <N>`    | Cap number of tabs (default 50).                                       |
| `--all`              | Open all changed files (no cap).                                       |
| `-h`, `--help`       | Show help.                                                             |
| `--version`          | Show version.                                                          |

### Default Vim executable

Resolution order:

1. `--vim <path>`
2. `$VIM_GIT_DIFF_VIM`
3. `gvim` on `PATH`
4. `gvim.exe` on `PATH` (Windows)
5. `vim` / `vim.exe` on `PATH`

If none are found, exits with `vim missing` (exit code 6).

### Binary files

Binary files (detected via `git diff --numstat`) are skipped with a stderr
message like:

```
skipping binary: assets/logo.png
```

### File cap

To keep large diffs manageable, vim-git-diff opens at most `--max-files`
(default 50) tabs and prints a stderr warning when truncation happens:

```
truncated to 50 of 137 files (use --all to override)
```

## How it works

For each changed file, a small Ex script tells Vim to:

1. Open a new tab and an empty `nofile` buffer.
2. Pull each side's content directly via `:read !git -C <repo> show <ref>:<path>`
   (or `:read <abspath>` for the working tree).
3. Detect the filetype using the file's name at that side.
4. Mark the buffer read-only and call `:diffthis`.

The script lives in a temp file that Vim deletes itself on exit via a
`VimLeave` autocmd. No blob content ever hits disk on our side.

## Exit codes

| Code | Meaning              |
| ---- | -------------------- |
| 0    | Success / no changes |
| 1    | Generic error        |
| 2    | Usage error          |
| 3    | Not a git repository |
| 4    | Bad revision         |
| 5    | `git` not found      |
| 6    | `vim`/`gvim` not found |

## Testing

```sh
dotnet test
```

All tests are pure and stub out git/process calls via interfaces, so they
run without a real Vim or git installation.

## License

MIT
