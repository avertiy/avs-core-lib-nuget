using System;
using System.IO;
using System.Text;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
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
                var sb = new StringBuilder(str);
                var i = 0;
                TagProcessor.Process(sb, (tagName, startIndex, endIndex) =>
                {
                    if (!Enum.TryParse(tagName, out CTag tag))
                        return -1;

                    var colors = tag.ToColors();
                    if (!colors.HasValue)
                        return -1;

                    var length = tagName.Length;
                    //sb.Replace($"</{tagName}>", string.Empty, endIndex - length - 3, length + 3);
                    //sb.Replace($"<{tagName}>", string.Empty, startIndex, length + 2);

                    if (startIndex > i)
                    {
                        var plainText = sb.ToString(i, startIndex - i);
                        Write(plainText, false);
                    }

                    var start = startIndex + length + 2;

                    var text = sb.ToString(start, endIndex-start-length-3);

                    Write(text, colors, false, false);
                    i = endIndex;
                    return length+2;
                });

                if (i < sb.Length)
                {
                    var text = sb.ToString(i, sb.Length - i);
                    Write(text,false);
                }

                if(endLine)
                    WriteLine();
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

            if(containsCTags.HasValue && containsCTags.Value)
                Write(str, endLine, containsCTags);
            else
                Write(str, endLine);

            // restore scheme
            PowerConsole.ApplyColorScheme(backup);
        }

        public void Write(string str, CTag tag, bool endLine)
        {
            var colors = tag.ToColors();
            if (!colors.HasValue)
            {
                Write(str, endLine);
            }
            else
            {
                Write(str, colors, endLine, false);
            }
        }

        #endregion
    }
}