using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Dates;

namespace AVS.CoreLib.Json;

public class UnixTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var seconds = reader.GetInt64();
        return DateTimeHelper.FromUnixTimestamp(seconds);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var unixTime = value.ToUnixTime();
        writer.WriteNumberValue(unixTime);
    }
}

//public class CustomDateTimeFormatConverter : JsonConverter<DateTime>
//{
//    private const string Format = "yyyy-MM-dd HH:mm:ss";

//    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        return DateTime.ParseExact(reader.GetString(), Format, CultureInfo.InvariantCulture);
//    }

//    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
//    {
//        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
//    }
//}