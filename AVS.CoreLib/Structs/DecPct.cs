using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Structs;

/// <summary>
/// Represents a pair of the decimal value and decimal pct (percentage) value  
/// e.g. pnl: { value:100$, pct: 0.01 } or price change: { value:10$, pct: 0.02 }
/// </summary>
[DebuggerDisplay("DecPct {ToString()}")]
[JsonConverter(typeof(DecPctJsonConverter))]
public readonly struct DecPct : IComparable<decimal>, IComparable<DecPct>, IEquatable<DecPct> //, IFormattable
{
    private readonly decimal _value;
    private readonly decimal _pctValue;

    public DecPct(decimal value, decimal pct)
    {
        _value = value;
        _pctValue = pct;
    }

    public decimal Value => _value.Round();

    /// <summary>
    /// represents a value in pct format, where 0.01 means 1%
    /// </summary>
    public decimal Pct => _pctValue.Round(4);

    /// <summary>
    /// represents value in percentage format, where 10 means 10%
    /// </summary>
    [JsonIgnore]
    public decimal Percent => Pct * 100;

    [JsonIgnore]
    public bool HasValue => _value != 0 && _pctValue !=0;
    /// <summary>
    /// returns an exact fraction value e.g. 0.2525218 (stored internally) => 0.2525218
    /// </summary>
    /// <returns></returns>
    public decimal GetExactValue() => _value;
    public decimal GetExactPctValue() => _pctValue;

    public override string ToString()
    {
        return _value == 0 ? "0" : $"{Value} ({Percent}%)";
    }
    public string ToString(string currency)
    {
        return _value == 0 ? $"0 {currency}" : $"{Value} {currency} ({Percent}%)";
    }

    //public string ToString(string? format, IFormatProvider? formatProvider)
    //{
    //    return _value == 0 ? $"0 {format}" : $"{Value} {format} ({Percent}%)";
    //}

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
                writer.WriteNumberValue(obj.Percent);
                break;
            case DecPctFormat.String:
                writer.WriteStringValue(obj.ToString());
                break;
            case DecPctFormat.Array:
                writer.WriteStartArray();
                writer.WriteNumberValue(obj.Value);
                writer.WriteNumberValue(obj.Pct);
                writer.WriteEndArray();
                break;
            default:
                JsonSerializer.Serialize(writer, obj, options);
                break;
        }
    }
}

//[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
//public class DecPctConverterAttribute : JsonConverterAttribute
//{
//    private readonly DecPctFormat _format;

//    public DecPctConverterAttribute(DecPctFormat format)
//    {
//        _format = format;
//    }

//    public override JsonConverter CreateConverter(Type typeToConvert)
//    {
//        return new DecPctJsonConverter(_format);
//    }
//}

public enum DecPctFormat
{
    /// <summary>
    /// serialize as object e.g. { "value": 0.0056, "pct": 0.061 }
    /// </summary>
    Default = 0,
    /// <summary>
    /// serialize as array: [0.0056, 0.061] i.e. compact
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

public static class DecPctExtensions
{
    public static DecPct DecPct(this decimal value, decimal mean, int? roundDecimals = null)
    {
        return mean == 0 ? default : new DecPct(value, (value / mean * 100m).Round(roundDecimals));
    }
}