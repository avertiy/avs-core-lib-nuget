namespace AVS.CoreLib.Logging.ColorFormatter.Enums;

public enum ObjType
{
    Null,
    Boolean,
    Byte,
    Integer,
    Float,
    String,
    DateTime,
    Time,
    Enum,
    Object,
    Array,
    List,
    Dictionary
}

[Flags]
public enum FormatFlags
{
    None = 0,
    Zero = 1,
    Negative =2,
    Currency =4,
    Percentage =8,
    ShortString =16,
    Text =32,
    Json =64,
    Brackets = 128,
    SquareBrackets = 256,
    CurlyBrackets = 512
}


public enum ArgType
{
    Default = 0,
    Null,
    Array,
    DateTime,
    Enum,
    Integer,
    IntegerNegative,
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
    TextJson
}