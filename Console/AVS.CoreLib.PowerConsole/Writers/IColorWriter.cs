using System;
using System.IO;
using System.Security.Cryptography;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public interface IColorWriter : IBasicWriter
    {
        void Write(string str, ConsoleColor? color, bool endLine, bool? containsCTags);
        void Write(string str, ColorScheme scheme, bool endLine, bool? containsCTags);
        void Write(string str, Colors colors, bool endLine, bool? containsCTags);
    }

    public class ColorWriter : BasicWriter, IColorWriter
    {
        public ColorWriter(TextWriter writer, TagProcessor? processor = null) : base(writer, processor ?? GetColorTagsProcessor())
        {
        }

        protected static TagProcessor GetColorTagsProcessor()
        {
            var tagProcessor = new CompositeTagProcessor();
            tagProcessor.AddTagProcessor(new CTagProcessor());
            tagProcessor.AddTagProcessor(new RgbTagProcessor());
            return tagProcessor;
        }


        #region Write methods

        public void Write(string str, ConsoleColor? color, bool endLine, bool? containsCTags)
        {
            if (color.HasValue)
            {
                var coloredStr = color.Value.Colorize(str);
                Write(coloredStr, endLine, containsCTags);
            }
            else
            {
                Write(str, endLine, containsCTags);
            }
        }

        public virtual void Write(string str, ColorScheme scheme, bool endLine, bool? containsCTags)
        {
            var coloredStr = scheme.Colorize(str);
            Write(coloredStr, endLine, containsCTags);
        }

        public virtual void Write(string str, Colors colors, bool endLine, bool? containsCTags)
        {
            var coloredStr = colors.Colorize(str);
            Write(coloredStr, endLine, containsCTags);
        }
        #endregion


        public static IColorWriter Create(TextWriter textWriter, ColorMode mode)
        {
            IColorWriter writer;
            switch (mode)
            {
                case ColorMode.PlainText:
                    writer = new PlainTextWriter(textWriter);
                    break;
                case ColorMode.Default:
                    writer = new ColorWriter(textWriter);
                    break;
                default:
                    writer = new SwitchColorWriter(textWriter);
                    break;
            }

            return writer;

        }
    }
}