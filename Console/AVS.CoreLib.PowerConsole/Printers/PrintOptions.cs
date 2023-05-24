using System;
using System.Diagnostics;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Printers
{
    /// <summary>
    /// Provide common parameters used in print methods,
    /// Options configure various print aspects like end line, colors, color tags etc.
    /// PrintOptions has various overloads for implicit conversion from tuples for usage convenience
    /// e.g. Print("str", (endLine: true, timeFormat:"G")),
    /// also predefined static props PrintOptions.Default, PrintOptions.Inline etc.
    /// of course you can pass argument explicitly e.g. Print("str", new PrintOptions() { ... }),
    /// </summary>
    public class PrintOptions
    {
        /// <summary>
        /// when not empty add timestamp prefix
        /// </summary>
        public string? TimeFormat { get; set; }

        public MessageLevel Level { get; set; }
        public bool EndLine { get; set; } = true;

        public bool? ColorTags { get; set; }
        public bool VoidEmptyLines { get; set; } = true;

        /// <summary>
        /// use color to colorize printed text in a certain color
        /// </summary>
        public ConsoleColor? Color { get; set; }
        /// <summary>
        /// use color scheme to colorize printed text (foreground/background colors)
        /// </summary>
        public ColorScheme? Scheme { get; set; }
        /// <summary>
        /// use colors to colorize printed text its quite similar to color scheme 
        /// </summary>
        public Colors? Colors { get; set; }

        /// <summary>
        /// use color palette to colorize <see cref="FormattableString"/> arguments in different colors
        /// </summary>
        public ColorPalette? ColorPalette { get; set; }

        public CTag? CTag { get; set; }

        public bool HasColors => Color.HasValue || Scheme.HasValue || Colors.HasValue || CTag.HasValue;

        public PrintOptions()
        {
        }

        public PrintOptions(bool endLine = true, bool colorTags = false, string? timeFormat = null, ConsoleColor? color = null)
        {
            EndLine = endLine;
            Color = color;
            ColorTags = colorTags;
            TimeFormat = timeFormat;
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

        public PrintOptions UseCTags()
        {
            ColorTags = true;
            return this;
        }

        public PrintOptions UseTimeStamp(string timeFormat = "HH:mm:ss")
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

        public virtual PrintOptions Clone(Action<PrintOptions>? configure = null)
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
            configure?.Invoke(copy);
            return copy;
        }

        #region implicit conversions
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
        #endregion

        #region implicit conversions from tuples

        [DebuggerStepThrough]
        public static implicit operator PrintOptions((bool endLine, bool colorTags) options)
        {
            return new PrintOptions() { EndLine = options.endLine, ColorTags = options.colorTags };
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions((bool endLine, bool colorTags, string timeFormat) options)
        {
            return new PrintOptions()
            {
                EndLine = options.endLine,
                ColorTags = options.colorTags,
                TimeFormat = options.timeFormat
            };
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions((bool endLine, bool colorTags, ConsoleColor color) options)
        {
            return new PrintOptions()
            {
                EndLine = options.endLine,
                ColorTags = options.colorTags,
                Color = options.color
            };
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions((bool endLine, ConsoleColor color) options)
        {
            return new PrintOptions() { EndLine = options.endLine, Color = options.color };
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions((bool endLine, string timeFormat) options)
        {
            return new PrintOptions()
            {
                EndLine = options.endLine,
                TimeFormat = options.timeFormat
            };
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions((bool endLine, string timeFormat, ConsoleColor color) options)
        {
            return new PrintOptions() { EndLine = options.endLine, TimeFormat = options.timeFormat, Color = options.color };
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions((bool endLine, bool colorTags, string timeFormat, ConsoleColor color) options)
        {
            return new PrintOptions()
            {
                EndLine = options.endLine,
                ColorTags = options.colorTags,
                TimeFormat = options.timeFormat,
                Color = options.color
            };
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions((bool endLine, MessageLevel level) options)
        {
            return new PrintOptions() { EndLine = options.endLine, Level = options.level };
        }

        [DebuggerStepThrough]
        public static implicit operator PrintOptions((bool endLine, ColorScheme scheme) options)
        {
            return new PrintOptions() { EndLine = options.endLine, Scheme = options.scheme };
        }

        #endregion

        
        public static explicit operator PrintOptions(bool endLine)
        {
            var options = new PrintOptions
            {
                EndLine = endLine
            };

            return options;
        }

        [DebuggerStepThrough]
        public static PrintOptions Create(bool endLine = true, bool colorTags = false, string? timeFormat = "HH:mm:ss", ConsoleColor? color = null)
        {
            var options = new PrintOptions { ColorTags = colorTags, EndLine = endLine, TimeFormat = timeFormat, Color = color };
            return options;
        }

        /// <summary>
        /// Instructs printer to treat message as containing color tags
        /// usage example:
        /// <code>
        /// PowerConsole.Print("no color <Red>text in red</Red>", PrintOptions.WithCTags(...));
        /// </code>
        /// </summary>
        public static PrintOptions WithCTags(bool endLine = true, string? timeFormat = "HH:mm:ss", ConsoleColor? color = null)
        {
            var options = new PrintOptions
            {
                ColorTags = true,
                EndLine = endLine,
                TimeFormat = timeFormat,
                Color = color
            };
            return options;
        }

        public static PrintOptions Default { get; set; } = new()
        {
            EndLine = true,
            TimeFormat = "HH:mm:ss",
            ColorTags = false
        };

        /// <summary>
        /// Use CTags options if the message contains color tags 
        /// usage example:
        /// <code>
        /// PowerConsole.Print("<Red>text in red</Red>", PrintOptions.CTags);
        /// </code>
        /// </summary>
        public static PrintOptions CTags { get; set; } = new()
        {
            EndLine = true,
            TimeFormat = "HH:mm:ss",
            ColorTags = true
        };

        /// <summary>
        /// Use Inline options when print should not put the end line break
        /// usage example:
        /// <code>
        /// PowerConsole.Print("some text", PrintOptions.Inline);
        /// PowerConsole.Print("continue text on the same line", PrintOptions.Inline);
        /// </code>
        /// </summary>
        public static PrintOptions Inline { get; set; } = new()
        {
            EndLine = false,
            TimeFormat = "HH:mm:ss",
            ColorTags = false
        };

        /// <summary>
        /// Use NoTimestamp options when print should not add timestamp label
        /// <code>
        /// PowerConsole.Print("some text", PrintOptions.NoTimestamp);
        /// </code>
        /// </summary>
        public static PrintOptions NoTimestamp { get; set; } = new()
        {
            EndLine = true,
            ColorTags = false
        };

        public static PrintOptions EmptyLinesVoided { get; set; } = new()
        {
            VoidEmptyLines = true,
            EndLine = true
        };

        public static PrintOptions EmptyLinesAllowed { get; set; } = new()
        {
            VoidEmptyLines = false,
            EndLine = true
        };

        public static PrintOptions NoTimestampInLine { get; set; } = new()
        {
            EndLine = false,
            ColorTags = false
        };

        public static PrintOptions Debug { get; set; } = new()
        {
            EndLine = true,
            TimeFormat = "HH:mm:ss",
            ColorTags = false,
            Level = MessageLevel.Debug,
            Color = ConsoleColor.DarkGray,
        };

        public static PrintOptions Error { get; set; } = new()
        {
            Color = ConsoleColor.Red,
            EndLine = true,
            TimeFormat = "HH:mm:ss",
            ColorTags = false,
            Level = MessageLevel.Error
        };

        /// <summary>
        /// Note: multiple colors works with PrintF methods it does not work with Print methods
        /// </summary>
        public static PrintOptions FromColors(params ConsoleColor[] colors)
        {
            return new PrintOptions()
            {
                ColorPalette = new ColorPalette(colors),
                EndLine = true,
            };
        }

        public static PrintOptions FromColors(ConsoleColor[] colors, string? timeFormat = null, bool endLine = true)
        {
            return new PrintOptions()
            {
                ColorPalette = new ColorPalette(colors),
                TimeFormat = timeFormat,
                EndLine = endLine,
            };
        }

        public static PrintOptions FromColorPalette(ColorPalette palette, string? timeFormat = null, bool endLine = true)
        {
            return new PrintOptions()
            {
                ColorPalette = palette,
                TimeFormat = timeFormat,
                EndLine = endLine,
            };
        }

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

    public static class PrintOptionsExtensions
    {
        public static Colors GetColors(this PrintOptions options)
        {
            if (options.Color.HasValue)
                return new Colors(options.Color, null);

            if (options.Colors.HasValue)
                return options.Colors.Value;

            if (options.Scheme.HasValue)
                return new Colors(options.Scheme.Value.Foreground, options.Scheme.Value.Background);

            if (options.CTag.HasValue)
                return options.CTag.Value.ToColors();

            return Colors.Empty;
        }
    }


    //todo rework print options to use enum flags, it would make options more simple

    public class HeaderPrintOptions : PrintOptions
    {
        public string Template { get; set; } = "============";
        public string LineIndentation { get; set; } = "\r\n";

        public static HeaderPrintOptions Options { get; set; } = new HeaderPrintOptions()
        {
            EndLine = true,
            Color = ConsoleColor.Cyan,
            Template = "============",
            LineIndentation = "\r\n",
        };

        public PrintOptions Clone()
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
}