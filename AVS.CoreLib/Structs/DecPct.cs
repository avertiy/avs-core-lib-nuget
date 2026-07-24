using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Structs;

/// <summary>
/// Represents a pair of the decimal value and percentage
/// The percentage part is optional in this case pct value is 0 so you operate only decimal value.
/// <code>
///  var pnl = new DecPct(100m, 0.02m); //  PNL=100$ (2%)
///  var change = new new DecPct(20m);// change=20$ we don't care about percentage
/// </code>
/// </summary>
[DebuggerDisplay("{ToString()}")]
[JsonConverter(typeof(DecPctJsonConverter))]
public readonly struct DecPct : IComparable<decimal>, IComparable<DecPct>, IEquatable<DecPct> //, IFormattable
{
    private readonly decimal _value;
    private readonly decimal _pctValue;

    public DecPct(decimal value, decimal pct = 0)
    {
        _value = value;
        _pctValue = pct;
    }

    /// <summary>
    /// returns rounded decimal value
    /// </summary>
    public decimal Value => _value.Round();

    /// <summary>
    /// returns rounded to 4 digitcs pct value e.g. 0.005 means 0.5%
    /// </summary>
    public decimal Pct => _pctValue.Round(4);

    [JsonIgnore]
    public bool HasValue => _value != 0;
    [JsonIgnore]
    public bool HasPctValue => _pctValue != 0;
    /// <summary>
    /// returns an exact fraction value e.g. 0.2525218 (stored internally) => 0.2525218
    /// </summary>
    /// <returns></returns>
    public decimal GetExactValue() => _value;
    public decimal GetExactPctValue() => _pctValue;

    /// <summary>
    /// returns rounded pct value as percentage e.g. 10.5 means 10.5% 
    /// </summary>
    public Percent GetPercent() => new Percent(Pct);

    public override string ToString()
    {
        return _pctValue == 0 ? Value.ToString() : $"{Value} ({Pct:P2})";
    }
    public string ToString(string currency)
    {
        return _pctValue == 0 ? $"{Value} {currency}" : $"{Value} {currency} ({Pct:P2})";
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (format != null && format.StartsWith("P"))
            return Pct.ToString(format, formatProvider);

        if (format != null && (format.StartsWith("C") || _pctValue == 0))
            return Value.ToString(format, formatProvider);

        return _pctValue == 0 ? Value.ToString() : $"{Value} ({Pct:P2})";
    }

    public int CompareTo(decimal other)
    {
        return _value.CompareTo(other);
    }

    public int CompareTo(DecPct other)
    {
        return _value.CompareTo(other._value);
    }

    public decimal Abs() => _value.Abs();

    public static implicit operator decimal(DecPct prop) => prop._value;
    public static implicit operator DecPct(decimal d) => new DecPct(d, 0);

    public static bool operator ==(DecPct a, DecPct b)
    {
        return a._value == b._value && a._pctValue == b._pctValue;
    }

    public static bool operator !=(DecPct a, DecPct b)
    {
        return !(a == b);
    }

    public static bool operator >(DecPct a, decimal b) => a._value > b;
    public static bool operator <(DecPct a, decimal b) => a._value < b;
    public static bool operator >=(DecPct a, decimal b) => a._value >= b;
    public static bool operator <=(DecPct a, decimal b) => a._value <= b;

    public static bool operator >(DecPct a, DecPct b) => a._value > b._value;
    public static bool operator <(DecPct a, DecPct b) => a._value < b._value;
    public static bool operator >=(DecPct a, DecPct b) => a._value >= b._value;
    public static bool operator <=(DecPct a, DecPct b) => a._value <= b._value;

    public bool Equals(DecPct other)
    {
        return _value == other._value && _pctValue == other._pctValue;
    }

    public override bool Equals(object? obj)
    {
        return obj is DecPct other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_value, _pctValue);
    }
}

public class DecPctJsonConverter : JsonConverter<DecPct>
{
    public DecPctFormat Format { get; set; } = DecPctFormat.Array;

    public override DecPct Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, DecPct obj, JsonSerializerOptions options)
    {
        switch (Format)
        {
            case DecPctFormat.Value:
                writer.WriteNumberValue(obj.Value);
                break;
            case DecPctFormat.Fraction:
                writer.WriteNumberValue(obj.Pct);
                break;
            case DecPctFormat.Percent:
                writer.WriteNumberValue(obj.Pct * 100);
                break;
            case DecPctFormat.String:
                writer.WriteStringValue(obj.ToString());
                break;
            case DecPctFormat.Array:
                if (obj.HasPctValue)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue(obj.Value);
                    writer.WriteNumberValue(obj.Pct);
                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteNumberValue(obj.Value);
                }
                break;

            default:
                if (obj.HasPctValue)
                {
                    // seialize as object e.g. "{value:100, pct: 0.02}";
                    writer.WriteStartObject();
                    writer.WritePropertyName("value");
                    writer.WriteNumberValue(obj.Value);
                    writer.WritePropertyName("pct");
                    writer.WriteNumberValue(obj.Pct);
                    writer.WriteEndObject();
                }
                else
                    // seialize as simple decimal e.g. "10.50"
                    writer.WriteNumberValue(obj.Value);
                break;
        }
    }
}

public enum DecPctFormat
{
    /// <summary>
    /// default serialization as object { "value": 0.0056, "pct": 0.061 }
    /// </summary>
    Default = 0,
    /// <summary>
    /// compact serializaion as array: [0.0056, 0.061]
    /// </summary>
    Array = 1,
    /// <summary>
    /// serialize value only, i.e. 0.0056 
    /// </summary>
    Value = 2,
    /// <summary>
    /// serialize fraction (pct) only, i.e. 0.061
    /// </summary>
    Fraction = 3,
    /// <summary>
    /// serialize percentage, i.e. "6.1"
    /// </summary>
    Percent = 4,
    /// <summary>
    /// serialize as ToString() e.g. "0.0056 (6.1%)"
    /// </summary>
    String = 5,

}

/// <summary>
/// uasge: [DecPctFormat(DecPctFormat.Default)]
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class DecPctFormatAttribute : JsonConverterAttribute
{
    public DecPctFormat Format { get; set; }

    public DecPctFormatAttribute(DecPctFormat format)
    {
        Format = format;
    }

    public override JsonConverter CreateConverter(Type typeToConvert)
    {
        return new DecPctJsonConverter() { Format = Format };
    }
}

public static class DecPctExtensions
{
    public static DecPct DecPct(this decimal value, decimal mean, int? roundDecimals = null)
    {
        return mean == 0 ? default : new DecPct(value, (value / mean * 100m).Round(roundDecimals));
    }
}