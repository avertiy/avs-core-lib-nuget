using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Json;

public class DecimalConverter : JsonConverter<decimal>
{
    private readonly int? _roundDecimals;

    public DecimalConverter(int? roundDecimals = null)
    {
        _roundDecimals = roundDecimals;
    }

    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDecimal();
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        var roundedValue = value.Round(_roundDecimals);
        writer.WriteNumberValue(roundedValue);
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class DecimalConverterAttribute : JsonConverterAttribute
{
    private readonly int? _roundDecimals;

    public DecimalConverterAttribute()
    {
        _roundDecimals = null;
    }

    public DecimalConverterAttribute(int roundDecimals)
    {
        _roundDecimals = roundDecimals;
    }

    public override JsonConverter CreateConverter(Type typeToConvert)
    {
        return new DecimalConverter(_roundDecimals);
    }
}