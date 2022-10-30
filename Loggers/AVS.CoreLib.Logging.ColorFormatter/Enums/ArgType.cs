namespace AVS.CoreLib.Logging.ColorFormatter.Enums;

public enum ArgType
{
    Default = 0,
    Array,
    Numeric,
    NumericNegative,
    Cash,
    CashNegative,
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