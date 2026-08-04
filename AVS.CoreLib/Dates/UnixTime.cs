using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Enums;

namespace AVS.CoreLib.Dates;

[DebuggerDisplay("{Timestamp} ({Unit}, {LocalDateTime})")]
[JsonConverter(typeof(UnixTimeJsonConverter))]
public readonly record struct UnixTime(long Timestamp) : IComparable<UnixTime>
{
    public static string DateTimeFormat { get; set; } = "u";// u - universal date-time: 2009-06-15 13:45:00Z 

    public DateTimeOffset Utc => DateTimeHelper.GetDateTimeOffset(Timestamp);
    public DateTimeOffset LocalDateTime => Utc.ToLocalTime();

    public TimeUnit Unit => DateTimeHelper.GetTimeUnit(Timestamp);

    public DayOfWeek DayOfWeek => Utc.Date.DayOfWeek;
    public DateTime Date => Utc.Date;

    public UnixTime(DateTime dateTime)
    : this(dateTime.ToUnixTimeMs())
    {
    }

    public UnixTime AddDays(int days)
    {
        
        checked
        {
            return Unit switch
            {
                TimeUnit.Seconds =>
                    new UnixTime(Timestamp + days * TimeSpan.SecondsPerDay),

                TimeUnit.Milliseconds =>
                    new UnixTime(Timestamp + days * TimeSpan.MillisecondsPerDay),

                TimeUnit.Microseconds =>
                    new UnixTime(Timestamp + days * TimeSpan.MicrosecondsPerDay),

                _ => throw new NotSupportedException($"Unsupported time unit: {Unit}")
            };
        }
    }

    public UnixTime AddSeconds(int seconds)
    {

        checked
        {
            return Unit switch
            {
                TimeUnit.Seconds =>
                    new UnixTime(Timestamp + seconds),

                TimeUnit.Milliseconds =>
                    new UnixTime(Timestamp + seconds * TimeSpan.MillisecondsPerSecond),

                TimeUnit.Microseconds =>
                    new UnixTime(Timestamp + seconds * TimeSpan.MicrosecondsPerSecond),

                _ => throw new NotSupportedException($"Unsupported time unit: {Unit}")
            };
        }
    }

    public UnixTime AddMilliSeconds(int millieseconds)
    {

        checked
        {
            return Unit switch
            {
                TimeUnit.Seconds =>
                    new UnixTime(Timestamp + millieseconds / 1000),

                TimeUnit.Milliseconds =>
                    new UnixTime(Timestamp + millieseconds),

                TimeUnit.Microseconds =>
                    new UnixTime(Timestamp + millieseconds * TimeSpan.MicrosecondsPerMillisecond),

                _ => throw new NotSupportedException($"Unsupported time unit: {Unit}")
            };
        }
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
    public static implicit operator UnixTime(long timestamp) => new (timestamp);
    public static implicit operator DateTimeOffset(UnixTime time) => time.Utc;
    public static implicit operator DateTime(UnixTime time) => DateTimeHelper.FromUnixTimestamp(time.Timestamp);

    public static bool operator >(UnixTime a, UnixTime b) => a.Timestamp > b.Timestamp;
    public static bool operator <(UnixTime a, UnixTime b) => a.Timestamp < b.Timestamp;
    public static bool operator >=(UnixTime a, UnixTime b) => a.Timestamp >= b.Timestamp;
    public static bool operator <=(UnixTime a, UnixTime b) => a.Timestamp <= b.Timestamp;

    public static bool TryParse(string str, out UnixTime time)
    {
        if(long.TryParse(str, out var timestamp))
        {
            time = new UnixTime(timestamp);
            return true;
        }

        time = default;
        return false;
    }

}

public class UnixTimeJsonConverter : JsonConverter<UnixTime>
{
    public JsonFormat Format { get; set; } = JsonFormat.Array;

    public override UnixTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out var timestamp))
                return new UnixTime(timestamp);

            var seconds = reader.GetDecimal();

            return new UnixTime((long)(seconds * 1000m));
        }
        else
        {
            var str = reader.GetString();
            if (long.TryParse(str, out var timestamp))
                return new UnixTime(timestamp);

            if (decimal.TryParse(str, out var dec))
                return new UnixTime((long)(dec * 1000m));

            if (DateTime.TryParse(str, out var dateTime))
                return new UnixTime(dateTime);

            throw new JsonException($"Invalid UnixTime value: {str}");
        }
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