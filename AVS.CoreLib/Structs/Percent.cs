using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Structs;

/// <summary>
/// Represents a decimal wrapper to deal with percent type
/// Helps to eliminate ambiguity whether it is fraction decimal or % decimal?
/// </summary>
[JsonConverter(typeof(PercentJsonConverter))]
[DebuggerDisplay("Percent {ToString()}")]
public struct Percent : IComparable<decimal>, IComparable<Percent>, IFormattable
{
    private decimal _value; // stored as 0.2525218 internally

    /// <summary>
    /// Rounded fraction value e.g. 0.2525218 (stored internally) => 0.2525
    /// </summary>
    public decimal Value
    {
        get => _value.Round();
        set => _value = value;
    }

    /// <summary>
    /// returns rounded % value e.g. 0.2525218 => 25.25%
    /// </summary>
    public decimal AsWhole(int decimals = 2) => (_value * 100).Round(decimals);
    /// <summary>
    /// returns rounded fraction value e.g. 0.2525218 (stored internally) => 0.2525
    /// </summary>
    /// <returns></returns>
    public decimal AsFraction() => Value;
    /// <summary>
    /// returns an exact fraction value e.g. 0.2525218 (stored internally) => 0.2525218
    /// </summary>
    /// <returns></returns>
    public decimal GetExactValue() => _value;

    public Percent(decimal value)
    {
        _value =  value;
    }


    public static implicit operator decimal(Percent number) => number._value;
    public static implicit operator Percent(decimal value)
    {
        if (value.Abs() > 1m)
            throw new ArgumentException("Implicit conversion expects fraction % value from [-1m;1m] e.g. 0.25m meaning 25%");
        return new Percent(value);
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

    public static Percent operator *(Percent a, Percent b) => new(a._value * b._value);
    public static Percent operator *(Percent a, decimal b) => new(a._value * b);
    public static Percent operator *(Percent a, int b) => new(a._value / b);
    public static decimal operator *(decimal a, Percent b) => a * b._value;
    public static decimal operator *(int a, Percent b) => a * b._value;

    public static Percent operator /(Percent a, Percent b) => new(a._value / b._value);
    public static Percent operator /(Percent a, decimal b) => new(a._value / b);
    public static Percent operator /(Percent a, int b) => new(a._value / b);
    public static decimal operator /(decimal a, Percent b) => a / b._value;
    public static decimal operator /(int a, Percent b) => a / b._value;

    public static Percent operator +(Percent a, Percent b) => new(a._value + b._value);
    public static Percent operator +(Percent a, decimal b) => new(a._value + b);
    public static Percent operator +(Percent a, int b) => new(a._value + b);
    public static decimal operator +(decimal a, Percent b) => a + b._value;
    public static decimal operator +(int a, Percent b) => a + b._value;

    public static Percent operator -(Percent a, Percent b) => new(a._value - b._value);
    public static Percent operator -(Percent a, decimal b) => new(a._value - b);
    public static Percent operator -(Percent a, int b) => new(a._value - b);
    public static decimal operator -(decimal a, Percent b) => a - b._value;
    public static decimal operator -(int a, Percent b) => a - b._value;

    public static bool operator > (Percent a, decimal b) => a._value > b;
    public static bool operator < (Percent a, decimal b) => a._value < b;
    public static bool operator >= (Percent a, decimal b) => a._value >= b;
    public static bool operator <= (Percent a, decimal b) => a._value <= b;

    public decimal SafeDivide(decimal divider) => _value.SafeDivide(divider);
    public Percent SafeDivide(Percent divider) => new(_value.SafeDivide(divider._value));

    public int CompareTo(decimal other)
    {
        return _value.CompareTo(other);
    }

    public int CompareTo(Percent other)
    {
        return _value.CompareTo(other._value);
    }

    public static Percent FromPct(decimal value)
    {
        return new Percent(value / 100m);
    }
}

public class PercentJsonConverter : JsonConverter<Percent>
{
    public override Percent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString() ?? string.Empty;

            // 1% means 1% so its fraction representation will be 0.01m
            if (str.EndsWith("%") && decimal.TryParse(str.TrimEnd('%'), out var value))
                return new Percent(value / 100);

            // otherwise treat as a normal fraction representation where 1 => 100% 
            return new Percent(decimal.Parse(str));
        }

        return new Percent(reader.GetDecimal());
    }

    public override void Write(Utf8JsonWriter writer, Percent number, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(number.Value);
    }
}

public static class DecimalPctExtensions
{
    public static Percent ToPercent(this decimal value, bool isFraction = true)
    {
        return isFraction ? new Percent(value) : new Percent(value/100);
    }

    public static Percent PctToPercent(this decimal value)
    {
        return new Percent(value / 100);
    }
}

