using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Structs;

/// <summary>
/// Represents a decimal wrapper to eliminate ambiguity when dealing with percentage values.
/// Pct means fraction representation e.g. 0.1 means 10% 
/// </summary>
[JsonConverter(typeof(PctJsonConverter))]
[DebuggerDisplay("Pct {ToString()}")]
public struct Pct : IComparable<decimal>, IComparable<Pct>, IFormattable
{
    private decimal _value; // stored internally as fraction e.g. 0.2525218 

    /// <summary>
    /// Rounded pct  value (fraction) e.g. 0.2525218 (stored internally) => 0.2525
    /// </summary>
    public decimal Value
    {
        get => _value.Round();
        set => _value = value;
    }

    [JsonIgnore]
    public decimal Percent => AsWhole();

    /// <summary>
    /// returns rounded % value e.g. 0.2525218 => 25.25%
    /// </summary>
    public decimal AsWhole(int decimals = 2) => (_value * 100).Round(decimals);
    /// <summary>
    /// returns rounded fraction value e.g. 0.2525218 (stored internally) => 0.2525
    /// </summary>
    public decimal AsFraction() => Value;
    /// <summary>
    /// returns an exact fraction value e.g. 0.2525218 (stored internally) => 0.2525218
    /// </summary>
    public decimal GetExactValue() => _value;

    public Pct(decimal value)
    {
        _value =  value;
    }


    public static implicit operator decimal(Pct number) => number._value;
    public static implicit operator Pct(decimal value)
    {
        Guard.MustBe.Fraction(value);
        return new Pct(value);
    }

    public bool IsEqual(decimal value, decimal tolerance) => _value.IsEqual(value, tolerance);

    public decimal Round(int? roundDecimals = null, int extraPrecision = 0, int minPrecision = 0) => _value.Round(roundDecimals, extraPrecision, minPrecision);

    public decimal Pow(double pow) => _value.Pow(pow);

    public decimal Sqrt() => _value.Sqrt();

    public decimal Abs() => _value.Abs();

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => $"{Value:P2}";
    public string ToStringPercentValue() => $"{Value:P2}";
    public string ToStringFractionValue() => Value.ToString(CultureInfo.InvariantCulture);
    public string ToStringExactValue() => _value.ToString(CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Value.ToString(format, formatProvider);
    }

    public string ToString(string format) => $"{Value.ToString(format)}";

    public string ToStringExactValue(string format) => $"{_value.ToString(format)}";

    public static Pct operator *(Pct a, Pct b) => new(a._value * b._value);
    public static Pct operator *(Pct a, decimal b) => new(a._value * b);
    public static Pct operator *(Pct a, int b) => new(a._value / b);
    public static decimal operator *(decimal a, Pct b) => a * b._value;
    public static decimal operator *(int a, Pct b) => a * b._value;

    public static Pct operator /(Pct a, Pct b) => new(a._value / b._value);
    public static Pct operator /(Pct a, decimal b) => new(a._value / b);
    public static Pct operator /(Pct a, int b) => new(a._value / b);
    public static decimal operator /(decimal a, Pct b) => a / b._value;
    public static decimal operator /(int a, Pct b) => a / b._value;

    public static Pct operator +(Pct a, Pct b) => new(a._value + b._value);
    public static Pct operator +(Pct a, decimal b) => new(a._value + b);
    public static Pct operator +(Pct a, int b) => new(a._value + b);
    public static decimal operator +(decimal a, Pct b) => a + b._value;
    public static decimal operator +(int a, Pct b) => a + b._value;

    public static Pct operator -(Pct a, Pct b) => new(a._value - b._value);
    public static Pct operator -(Pct a, decimal b) => new(a._value - b);
    public static Pct operator -(Pct a, int b) => new(a._value - b);
    public static decimal operator -(decimal a, Pct b) => a - b._value;
    public static decimal operator -(int a, Pct b) => a - b._value;

    public static bool operator > (Pct a, decimal b) => a._value > b;
    public static bool operator < (Pct a, decimal b) => a._value < b;
    public static bool operator >= (Pct a, decimal b) => a._value >= b;
    public static bool operator <= (Pct a, decimal b) => a._value <= b;

    public decimal SafeDivide(decimal divider) => _value.SafeDivide(divider);
    public Pct SafeDivide(Pct divider) => new(_value.SafeDivide(divider._value));

    public int CompareTo(decimal other)
    {
        return _value.CompareTo(other);
    }

    public int CompareTo(Pct other)
    {
        return _value.CompareTo(other._value);
    }

    public static Pct FromPct(decimal value)
    {
        return new Pct(value / 100m);
    }
}

public class PctJsonConverter : JsonConverter<Pct>
{
    public override Pct Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString() ?? string.Empty;

            // 1% means 1% so its fraction representation will be 0.01m
            if (str.EndsWith("%") && decimal.TryParse(str.TrimEnd('%'), out var value))
                return new Pct(value / 100);

            // otherwise treat as a normal fraction representation where 1 => 100% 
            return new Pct(decimal.Parse(str));
        }

        return new Pct(reader.GetDecimal());
    }

    public override void Write(Utf8JsonWriter writer, Pct number, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(number.Value);
    }
}

public static class DecimalPctExtensions
{
    public static Pct ToPct(this decimal value, bool isFraction = true)
    {
        return isFraction ? new Pct(value) : new Pct(value / 100);
    }
}