using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Enums;
using AVS.CoreLib.Structs;

namespace AVS.CoreLib.Dates;

[DebuggerDisplay("{Timestamp} ({Units}, {LocalDateTime})")]
[JsonConverter(typeof(UnixTimeJsonConverter))]
public readonly record struct UnixTime(long Timestamp) : IComparable<UnixTime>
{
    public static string DateTimeFormat { get; set; } = "u";// u - universal date-time: 2009-06-15 13:45:00Z 

    public DateTimeOffset Utc =>
        DateTimeOffset.FromUnixTimeMilliseconds(Timestamp);
    public DateTimeOffset LocalDateTime => Utc.ToLocalTime();

    public TimeUnit Unit => DateTimeHelper.GetTimeUnit(Timestamp);

    public DayOfWeek DayOfWeek => Utc.Date.DayOfWeek;
    public DateTime Date => Utc.Date;

    public UnixTime(DateTime dateTime)
    : this(dateTime.ToUnixTimeMs())
    {
    }

    public override string ToString()
        => LocalDateTime.ToString(DateTimeFormat);

    public string ToString(string format)
        => LocalDateTime.ToString(format);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format))
            return LocalDateTime.ToString(format, formatProvider);

        return LocalDateTime.ToString(DateTimeFormat);
    }

    public int CompareTo(UnixTime other)
    {
        return Timestamp.CompareTo(other.Timestamp);
    }

    public static implicit operator long(UnixTime time) => time.Timestamp;
    public static implicit operator DateTimeOffset(UnixTime time) => time.Utc;
    public static implicit operator DateTime(UnixTime time) => DateTimeHelper.FromUnixTimestamp(time.Timestamp);

    public static bool operator >(UnixTime a, UnixTime b) => a.Timestamp > b.Timestamp;
    public static bool operator <(UnixTime a, UnixTime b) => a.Timestamp < b.Timestamp;
    public static bool operator >=(UnixTime a, UnixTime b) => a.Timestamp >= b.Timestamp;
    public static bool operator <=(UnixTime a, UnixTime b) => a.Timestamp <= b.Timestamp;

}

public class UnixTimeJsonConverter : JsonConverter<UnixTime>
{
    public JsonFormat Format { get; set; } = JsonFormat.Array;

    public override UnixTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var timestamp = reader.GetInt64();
        return new UnixTime(timestamp);
    }

    public override void Write(Utf8JsonWriter writer, UnixTime obj, JsonSerializerOptions options)
    {
        switch (Format)
        {
            case JsonFormat.Array:
                    writer.WriteStartArray();
                    writer.WriteNumberValue(obj.Timestamp);
                    writer.WriteStringValue(obj.LocalDateTime.ToString(UnixTime.DateTimeFormat));
                    writer.WriteEndArray();
                break;
            case JsonFormat.String:
                writer.WriteStringValue(obj.ToString());
                break;
            case JsonFormat.Value:
            default:
                writer.WriteNumberValue(obj.Timestamp);
                break;                
        }
    }
}