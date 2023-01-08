using System;
using System.Diagnostics;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Printers
{
    /// <summary>
    /// Print options provide common parameters used in print methods, allows to set all the various aspects within one argument instead of having multiple method overloads 
    /// </summary>
    public class PrintOptions
    {
        /// <summary>
        /// when not empty add timestamp prefix
        /// </summary>
        public string? TimeFormat { get; set; }

        public MessageLevel Level { get; set; }
        public bool EndLine { get; set; }

        public bool? ColorTags { get; set; }
        public bool VoidEmptyLines { get; set; } = true;

        /// <summary>
        /// color for printed text
        /// </summary>
        public ConsoleColor? Color { get; set; }
        /// <summary>
        /// color scheme for printed text
        /// </summary>
        public ColorScheme? Scheme { get; set; }
        /// <summary>
        /// colors for printed text
        /// </summary>
        public Colors? Colors { get; set; }

        public CTag? CTag { get; set; }

        public bool HasColors => Color.HasValue || Scheme.HasValue || Colors.HasValue || CTag.HasValue;

        public PrintOptions(bool endLine = true, bool colorTags = false, string? timeFormat = null, ConsoleColor? color = null)
        {
            EndLine = endLine;
            Color = color;
            ColorTags = colorTags;
            TimeFormat = timeFormat;
        }


        public Colors GetColors()
        {
            if (Color.HasValue)
                return new Colors(Color, null);
            if (Colors.HasValue)
                return this.Colors.Value;

            if (Scheme.HasValue)
                return new Colors(Scheme.Value.Foreground, Scheme.Value.Background);

            if (CTag.HasValue)
                return CTag.Value.ToColors();

            return AVS.CoreLib.Console.ColorFormatting.Colors.Empty;
        }

        #region UseXXX methods
        public PrintOptions UseCTag(CTag tag)
        {
            CTag = tag;
            return this;
        }

        public PrintOptions UseColor(ConsoleColor color)
        {
            Color = color;
            return this;
        }

        public PrintOptions UseColorScheme(ColorScheme scheme)
        {
            Scheme = scheme;
            return this;
        }

        public PrintOptions UseColors(Colors colors)
        {
            Colors = colors;
            return this;
        }

        public PrintOptions WithCTags()
        {
            ColorTags = true;
            return this;
        }

        public PrintOptions UseTimeStamp(string timeFormat = "hh:mm:ss")
        {
            TimeFormat = timeFormat;
            return this;
        }

        public PrintOptions NoEndLine()
        {
            EndLine = false;
            return this;
        }

        public PrintOptions NoTimeStamp()
        {
            TimeFormat = null;
            return this;
        } 
        #endregion

        public virtual PrintOptions Clone()
        {
            var copy = new PrintOptions()
            {
                EndLine = EndLine,
                Color = Color,
                Scheme = Scheme,
                ColorTags = ColorTags,
                TimeFormat = TimeFormat,
                Level = Level,
                VoidEmptyLines = VoidEmptyLines
            };

            return copy;
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions(ConsoleColor color)
        {
            return new PrintOptions().UseColor(color);
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions(CTag tag)
        {
            return new PrintOptions().UseCTag(tag);
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions(ColorScheme scheme)
        {
            return new PrintOptions().UseColorScheme(scheme);
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions(Colors colors)
        {
            return new PrintOptions().UseColors(colors);
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions(MessageLevel level)
        {
            var options = new PrintOptions
            {
                Level = level,
                Scheme = ColorScheme.GetColorScheme(level)
            };

            return options;
        }

        public static explicit operator PrintOptions(bool endLine)
        {
            var options = new PrintOptions
            {
                EndLine = endLine
            };

            return options;
        }

        public static PrintOptions Default { get; set; } = new PrintOptions()
        {
            EndLine = true,
            TimeFormat = "hh:mm:ss",
            ColorTags = false
        };

        [DebuggerStepThrough]
        public static PrintOptions Create(bool endLine = true, bool colorTags = false, string? timeFormat = "hh:mm:ss", ConsoleColor? color = null)
        {
            var options = new PrintOptions { ColorTags = colorTags, EndLine = endLine, TimeFormat = timeFormat, Color = color };
            return options;
        }

        [DebuggerStepThrough]
        public static PrintOptions CTags()
        {
            var options = Default.Clone();
            options.ColorTags = true;
            return options;
        }

        [DebuggerStepThrough]
        public static PrintOptions CTags(bool endLine, string? timeFormat = "hh:mm:ss", ConsoleColor? color = null)
        {
            var options = new PrintOptions { ColorTags = true, EndLine = endLine, TimeFormat = timeFormat, Color = color};
            return options;
        } 

        public static PrintOptions Inline { get; set; } = new PrintOptions()
        {
            EndLine = false,
            TimeFormat = null,
            ColorTags = false
        };

        public static PrintOptions Debug { get; set; } = new PrintOptions()
        {
            EndLine = true,
            TimeFormat = "hh:mm:ss",
            ColorTags = false,
            Level = MessageLevel.Debug,
            Color = ConsoleColor.DarkGray,
        };

        public static PrintOptions Error { get; set; } = new PrintOptions()
        {
            Color = ConsoleColor.Red,
            EndLine = true,
            TimeFormat = "hh:mm:ss",
            ColorTags = false,
            Level = MessageLevel.Error
        };

        public static PrintOptions FromColor(ConsoleColor color, string? timeFormat = null, bool endLine = true)
        {
            return new PrintOptions()
            {
                Color = color,
                TimeFormat = timeFormat,
                EndLine = endLine,
            };
        }

        public static PrintOptions FromColorScheme(ColorScheme scheme, string? timeFormat = null, bool endLine = true)
        {
            return new PrintOptions()
            {
                Scheme = scheme,
                TimeFormat = timeFormat,
                EndLine = endLine,
            };
        }
    }

    public class HeaderPrintOptions : PrintOptions
    {
        public string Template { get; set; } = "============";
        public string LineIndentation { get; set; } = "\r\n\r\n";

        public static HeaderPrintOptions Options { get; set; } = new HeaderPrintOptions()
        {
            EndLine = true,
            Color = ConsoleColor.Cyan,
            Template = "============",
            LineIndentation = "\r\n\r\n",
        };

        public override PrintOptions Clone()
        {
            var copy = new HeaderPrintOptions()
            {
                LineIndentation = LineIndentation,
                Template = Template,
                EndLine = EndLine,
                Color = Color,
                Scheme = Scheme,
                ColorTags = ColorTags,
                TimeFormat = TimeFormat,
                Level = Level,
                VoidEmptyLines = VoidEmptyLines
            };

            return copy;
        }
    }

    /// <summary>
    /// colorize arguments of <see cref="FormattableString"/>
    /// similar to auto-highlight feature in color formatter for console logging
    /// </summary>
    public class MultiColorPrintOptions : PrintOptions
    {
        public ColorPalette Palette { get; set; }

        public MultiColorPrintOptions(bool endLine = true)
        {
            Palette = new ColorPalette();
            EndLine = endLine;
        }

        public MultiColorPrintOptions(ConsoleColor[] colors, bool endLine = true)
        {
            Palette = new ColorPalette(colors);
            EndLine = endLine;
        }

        public static implicit operator MultiColorPrintOptions(ColorPalette palette)
        {
            return new MultiColorPrintOptions() { Palette = palette };
        }

        public static implicit operator MultiColorPrintOptions(ConsoleColor[] colors)
        {
            return new MultiColorPrintOptions(colors);
        }
    }
}