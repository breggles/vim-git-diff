using System.Text;

namespace VimGitDiff.Vim;

public static class VimShellQuoter
{
    public static string QuoteForCmd(string arg)
    {
        var sb = new StringBuilder();

        sb.Append('"');

        foreach (var ch in arg)
        {
            if (ch == '"')
            {
                sb.Append("\\\"");
            }
            else
            {
                sb.Append(ch);
            }
        }

        sb.Append('"');

        return sb.ToString();
    }

    public static string QuoteForPosix(string arg)
    {
        var sb = new StringBuilder();

        sb.Append('\'');

        foreach (var ch in arg)
        {
            if (ch == '\'')
            {
                sb.Append("'\\''");
            }
            else
            {
                sb.Append(ch);
            }
        }

        sb.Append('\'');

        return sb.ToString();
    }
}
