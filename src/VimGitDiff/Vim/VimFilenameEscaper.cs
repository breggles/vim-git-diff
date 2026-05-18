using System.Collections.Generic;
using System.Text;

namespace VimGitDiff.Vim;

public static class VimFilenameEscaper
{
    private static readonly HashSet<char> EscapeChars = new(" \t\n\r%#|\\\"'<>$&*?[]{}();`!");

    public static string Escape(string label)
    {
        var sb = new StringBuilder(label.Length);

        foreach (var ch in label)
        {
            if (EscapeChars.Contains(ch) || ch < 0x20)
            {
                sb.Append('\\');

                sb.Append(ch);
            }
            else
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }
}
