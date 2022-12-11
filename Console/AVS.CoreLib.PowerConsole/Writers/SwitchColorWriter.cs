using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.TagProcessors;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class SwitchColorWriter : BasicWriter, IColorWriter
    {
        public SwitchColorWriter(TextWriter writer) : base(writer, new DummyTagProcessor())
        {
        }

        #region Write methods

        public void Write(string str, ConsoleColor? color, bool endLine, bool? containsCTags)
        {
            if (color.HasValue)
            {
                var colorBackup = System.Console.ForegroundColor;
                System.Console.ForegroundColor = color.Value;
                Write(str, endLine, containsCTags);
                System.Console.ForegroundColor = colorBackup;
            }
            else
            {
                Write(str, endLine, containsCTags);
            }
        }

        public override void Write(string str, bool endLine, bool? containsCTags)
        {
            if (containsCTags.HasValue && containsCTags.Value)
            {
                //var str = new CTagMarkupString(str);

                throw new NotImplementedException();
            }
            else
            {
                Write(str, endLine);
            }
        }

        public virtual void Write(string str, ColorScheme scheme, bool endLine, bool? containsCTags)
        {
            var backup = ColorScheme.GetCurrentScheme();
            PowerConsole.ApplyColorScheme(scheme);

            var coloredStr = scheme.Colorize(str);
            Write(coloredStr, endLine, containsCTags);

            // restore scheme
            PowerConsole.ApplyColorScheme(backup);
        }

        public virtual void Write(string str, Colors colors, bool endLine, bool? containsCTags)
        {
            var backup = ColorScheme.GetCurrentScheme();
            PowerConsole.ApplyColors(colors);

            var coloredStr = colors.Colorize(str);
            Write(coloredStr, endLine, containsCTags);

            // restore scheme
            PowerConsole.ApplyColorScheme(backup);
        }
        #endregion
    }
}