using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;
// ReSharper disable ArrangeMethodOrOperatorBody
namespace AVS.CoreLib.Structs;

/// <summary>
/// Represents a decimal wrapper
/// Allows to deal with a rounded decimal value, at the same time preserving an exact value for calculations
/// </summary>
[JsonConverter(typeof(NumberJsonConverter))]
[DebuggerDisplay("DecNumber {Value}")]
public struct DecNumber : IComparable<decimal>, IComparable<DecNumber>, IFormattable
{
    private decimal _value;

    public DecNumber(decimal value)
    {
        _value = value;
    }

    public decimal Value
    {
        get => _value.RoundPrice();
        set => _value = value;
    }

    public decimal GetExactValue() => _value;

    public static implicit operator decimal(DecNumber number) => number._value;
    public static implicit operator DecNumber(decimal value) => new(value);

    public bool IsEqual(decimal value, decimal tolerance) => _value.IsEqual(value, tolerance);

    public decimal Round(int? roundDecimals = null, int extraPrecision= 0, int minPrecision = 0) => _value.Round(roundDecimals, extraPrecision, minPrecision);

    public decimal Abs() => _value.Abs();

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Value.ToString(format, formatProvider);
    }

    public string ToString(string format) => Value.ToString(format);

    public string ToStringExactValue(string format) => _value.ToString(format);

    public static DecNumber operator *(DecNumber a, DecNumber b) => new(a._value * b._value);
    public static DecNumber operator *(DecNumber a, decimal b) => new(a._value * b);
    public static decimal operator *(decimal a, DecNumber b) => a * b._value;

    public static DecNumber operator /(DecNumber a, DecNumber b) => new(a._value / b._value);
    public static DecNumber operator / (DecNumber a, decimal b) => new(a._value / b);
    public static decimal operator / (decimal a, DecNumber b) => a / b._value;

    public static DecNumber operator + (DecNumber a, DecNumber b) => new(a._value + b._value);
    public static DecNumber operator +(DecNumber a, decimal b) => new(a._value + b);
    public static decimal operator +(decimal a, DecNumber b) => a + b._value;

    public static DecNumber operator -(DecNumber a, DecNumber b) => new(a._value - b._value);
    public static DecNumber operator -(DecNumber a, decimal b) => new(a._value - b);
    public static decimal operator -(decimal a, DecNumber b) => a - b._value;

    public decimal SafeDivide(decimal divider) => _value.SafeDivide(divider);
    public DecNumber SafeDivide(DecNumber divider) => new(_value.SafeDivide(divider._value));

    public int CompareTo(decimal other)
    {
        return _value.CompareTo(other);
    }

    public int CompareTo(DecNumber other)
    {
        return _value.CompareTo(other._value);
    }
}

public class NumberJsonConverter : JsonConverter<DecNumber>
{
    public override DecNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new DecNumber(reader.GetDecimal());
    }

    public override void Write(Utf8JsonWriter writer, DecNumber number, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(number.GetExactValue());
    }
}