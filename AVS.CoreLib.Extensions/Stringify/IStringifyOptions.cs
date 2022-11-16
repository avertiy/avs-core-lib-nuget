namespace AVS.CoreLib.Extensions.Stringify;

public interface IStringifyOptions
{
    string Separator { get; set; }
    string KeyValueSeparator { get; set; }
    StringifyFormat Format { get; set; }
    int MaxLength { get; set; }
}

public class StringifyOptions : IStringifyOptions
{
    public int MaxLength { get; set; }
    public string Separator { get; set; } = ",";
    public string KeyValueSeparator { get; set; } = ":";
    public StringifyFormat Format { get; set; } = StringifyFormat.Default;

    public static StringifyOptions Default = new StringifyOptions() { MaxLength = 256 };
}