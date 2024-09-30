namespace AVS.CoreLib.Logging.ColorFormatter.Enums;

public enum ObjType
{
    Null,
    Boolean,
    Byte,
    Number,
    Float,
    String,
    DateTime,
    Time,
    Enum,
    Object,
    Array,
    List,
    Dictionary,
    Enumerable
}

[Flags]
public enum FormatFlags
{
    None = 0,
    Zero = 1,
    Negative = 2,
    Currency = 4,
    Percentage = 8,
    ShortString = 16,
    Text = 32,
    Json = 64,
    Brackets = 128,
    SquareBrackets = 256,
    CurlyBrackets = 512,
}

[Flags]
public enum NumberFlags
{
    None = 0,
    Negative = 1,
    Zero = 2,
    Percentage = 4,
    Currency = 8,
    Float = 16,
    Count = 32
}

public enum TextKind
{
    /// <summary>
    /// just some string value
    /// </summary>
    None = 0,
    /// <summary>
    /// long string or text value
    /// </summary>
    Text = 1,
    Json = 2,
    Array = 3,
    /// <summary>
    /// value in brackets e.g. (str)
    /// </summary>
    Brackets,
    Percentage,
    //DateTime,
    Quotes,
    DoubleQuotes,
    Url,
    FilePath,
    HttpVerb,
    Symbol,
    /// <summary>
    /// keywords like Start/End
    /// </summary>
    Keyword,
    OK,
    Error,
    SpecialKeyword
}