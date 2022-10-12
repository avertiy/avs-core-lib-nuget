namespace AVS.CoreLib.Logging.ColorFormatter.Utils;
public interface IColorsProvider
{
    ConsoleColors GetColorsForArgument(ArgumentType kind);
    ConsoleColors GetColorsFor(LogParts part);
}

/// <summary>
/// bad naming i know think on better name...
/// </summary>
public enum ArgumentType
{
    Default = 0,
    Array,
    Numeric,
    NumericNegative,
    Cash,
    Percentage,
    /// <summary>
    /// short string
    /// </summary>
    String,
    /// <summary>
    /// long string
    /// </summary>
    Text,
    Date,
    TextJson
}

public enum LogParts
{
    Message = 0,
    Error,
    Scope,
    Category
}

public class ColorsProvider : IColorsProvider
{
    public ConsoleColors GetColorsForArgument(ArgumentType kind)
    {
        return kind switch
        {
            ArgumentType.Array => new ConsoleColors(ConsoleColor.Cyan, null),
            ArgumentType.Numeric => new ConsoleColors(ConsoleColor.DarkYellow, null),
            ArgumentType.NumericNegative => new ConsoleColors(ConsoleColor.Red, null),
            ArgumentType.Date => new ConsoleColors(ConsoleColor.Yellow, null),
            ArgumentType.Percentage => new ConsoleColors(ConsoleColor.Magenta, null),
            ArgumentType.Cash => new ConsoleColors(ConsoleColor.DarkGreen, null),
            ArgumentType.Text => new ConsoleColors(ConsoleColor.DarkGray, null),
            ArgumentType.TextJson => new ConsoleColors(ConsoleColor.Cyan, null),
            ArgumentType.Default => new ConsoleColors(ConsoleColor.Gray, null),
            _ => new ConsoleColors(ConsoleColor.Gray, null)
        };
    }

    public ConsoleColors GetColorsFor(LogParts part)
    {
        return part switch
        {
            LogParts.Error => new ConsoleColors(ConsoleColor.DarkRed, null),
            LogParts.Scope => new ConsoleColors(ConsoleColor.Cyan, null),
            LogParts.Category => new ConsoleColors(ConsoleColor.DarkYellow, null),
            _ => new ConsoleColors(ConsoleColor.Gray, null)
        };
    }
}