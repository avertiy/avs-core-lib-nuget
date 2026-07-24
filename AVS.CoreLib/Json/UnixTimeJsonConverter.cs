using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Dates;

namespace AVS.CoreLib.Json;

public class UnixTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var timestamp = reader.GetInt64();
        return DateTimeHelper.FromUnixTimestamp(timestamp);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var unixTime = value.ToUnixTime();
        writer.WriteNumberValue(unixTime);
    }
}
