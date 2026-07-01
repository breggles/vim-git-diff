using System;
using System.IO;

namespace VimGitDiff.Io;

public sealed class TempContentFile : IDisposable
{
    private bool _disposed;

    public string Path { get; }

    private TempContentFile(string path)
    {
        Path = path;
    }

    public static TempContentFile Create(byte[] content)
    {
        var dir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "vim-git-diff");

        Directory.CreateDirectory(dir);

        var name = $"vgd-{System.Environment.ProcessId}-{Guid.NewGuid():N}.content";

        var fullPath = System.IO.Path.Combine(dir, name);

        File.WriteAllBytes(fullPath, content);

        return new TempContentFile(fullPath);
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
