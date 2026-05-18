using System;
using System.IO;
using System.Text;

namespace VimGitDiff.Io;

public sealed class TempScriptFile : IDisposable
{
    private bool _disposed;

    public string Path { get; }

    private TempScriptFile(string path)
    {
        Path = path;
    }

    public static TempScriptFile Create(string content)
    {
        var dir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "vim-git-diff");

        Directory.CreateDirectory(dir);

        var name = $"vgd-{System.Environment.ProcessId}-{Guid.NewGuid():N}.vim";

        var fullPath = System.IO.Path.Combine(dir, name);

        File.WriteAllText(fullPath, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        return new TempScriptFile(fullPath);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
