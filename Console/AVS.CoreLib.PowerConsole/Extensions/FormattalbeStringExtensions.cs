using System;
using System.Text;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Text;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class FormattalbeStringExtensions
    {
        /// <summary>
        /// wrap arguments in color tags 
        /// </summary>
        public static FormattableString2 Colorize(this FormattableString str, params ConsoleColor[] colors)
        {
            var sb = new StringBuilder(str.Format);
            for (var i = 0; i < str.ArgumentCount; i++)
            {
                var ind = sb.IndexOfArg(i, out var len);
                if (ind == -1)
                    continue;

                var color = i <= colors.Length ? colors[i].ToString() : colors[i % colors.Length].ToString();

                sb.Insert(ind + len, $"</{color}>");
                sb.Insert(ind, $"<{color}>");

            }

            return new FormattableString2(sb.ToString(), str.GetArguments());
        }

        private static int IndexOfArg(this StringBuilder sb, int i, out int length)
        {
            length = 0;
            var arg = "{" + i + "}";
            var ind = sb.IndexOf(arg);

            if (ind > -1)
            {
                length = arg.Length;
                return ind;
            }

            arg = "{" + i + ":";
            ind = sb.IndexOf(arg);

            if (ind == -1)
                return -1;

            var indClosingBracket = sb.IndexOf('}', ind + 3);

            if (indClosingBracket == -1)
                return -1;

            length = indClosingBracket - ind + 1;
            return ind;
        }


    }
}