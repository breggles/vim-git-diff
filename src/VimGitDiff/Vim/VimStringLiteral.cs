using System.Text;

namespace VimGitDiff.Vim;

public static class VimStringLiteral
{
    public static string Quote(string value)
    {
        var sb = new StringBuilder();

        sb.Append('\'');

        foreach (var ch in value)
        {
            if (ch == '\'')
            {
                sb.Append("''");
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
