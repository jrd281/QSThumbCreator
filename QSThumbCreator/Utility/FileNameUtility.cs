using System.Text;
using System.Text.RegularExpressions;

namespace QSThumbCreator.Utility
{
    public class FileNameUtility
    {
        // <summary>
        // This will replace invalid chars with underscores, there are also some reserved words that it adds underscore to
        // </summary>
        // <remarks>
        // https://stackoverflow.com/questions/1976007/what-characters-are-forbidden-in-windows-and-linux-directory-names
        // </remarks>
        // <param name="containsFolder">Pass in true if filename represents a folder\file (passing true will allow slash)</param>
        public static string EscapeFilenameWindows(string filename, bool containsFolder = false)
        {
            StringBuilder builder = new StringBuilder(filename.Length + 12);

            var index = 0;

            // Allow colon if it's part of the drive letter
            if (containsFolder)
            {
                Match match = Regex.Match(filename, @"^\s*[A-Z]:\\", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    builder.Append(match.Value);
                    index = match.Length;
                }
            }

            // Character substitutions
            for (int cntr = index; cntr < filename.Length; cntr++)
            {
                char c = filename[cntr];

                switch (c)
                {
                    case '\u0000':
                    case '\u0001':
                    case '\u0002':
                    case '\u0003':
                    case '\u0004':
                    case '\u0005':
                    case '\u0006':
                    case '\u0007':
                    case '\u0008':
                    case '\u0009':
                    case '\u000A':
                    case '\u000B':
                    case '\u000C':
                    case '\u000D':
                    case '\u000E':
                    case '\u000F':
                    case '\u0010':
                    case '\u0011':
                    case '\u0012':
                    case '\u0013':
                    case '\u0014':
                    case '\u0015':
                    case '\u0016':
                    case '\u0017':
                    case '\u0018':
                    case '\u0019':
                    case '\u001A':
                    case '\u001B':
                    case '\u001C':
                    case '\u001D':
                    case '\u001E':
                    case '\u001F':

                    case '<':
                    case '>':
                    case ':':
                    case '"':
                    case '/':
                    case '|':
                    case '?':
                    case '*':
                        builder.Append('_');
                        break;

                    case '\\':
                        builder.Append(containsFolder ? c : '_');
                        break;

                    default:
                        builder.Append(c);
                        break;
                }
            }

            string built = builder.ToString();

            if (built == "")
            {
                return "_";
            }

            if (built.EndsWith(" ") || built.EndsWith("."))
            {
                built = built.Substring(0, built.Length - 1) + "_";
            }

            // These are reserved names, in either the folder or file name, but they are fine if following a dot
            // CON, PRN, AUX, NUL, COM0 .. COM9, LPT0 .. LPT9
            builder = new StringBuilder(built.Length + 12);
            index = 0;
            foreach (Match match in Regex.Matches(built, @"(^|\\)\s*(?<bad>CON|PRN|AUX|NUL|COM\d|LPT\d)\s*(\.|\\|$)",
                RegexOptions.IgnoreCase))
            {
                Group group = match.Groups["bad"];
                if (group.Index > index)
                {
                    builder.Append(built.Substring(index, match.Index - index + 1));
                }

                builder.Append(group.Value);
                builder.Append("_"); // putting an underscore after this keyword is enough to make it acceptable

                index = group.Index + group.Length;
            }

            if (index == 0)
            {
                return built;
            }

            if (index < built.Length - 1)
            {
                builder.Append(built.Substring(index));
            }

            return builder.ToString();
        }
    }
}