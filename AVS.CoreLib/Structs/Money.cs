using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;
namespace AVS.CoreLib.Structs;

/// <summary>
/// Represents a decimal wrapper for monetary values.
/// Allows to deal with a rounded to 2-3 digits value, preserving an exact value for precise calculations
/// </summary>
[JsonConverter(typeof(MoneyJsonConverter))]
[DebuggerDisplay("{Value}")]
public struct Money : IComparable<decimal>, IComparable<Money>, IComparable<DecNumber>, IFormattable
{
    private decimal _value;

    public Money(decimal value)
    {
        _value = value;
    }

    public decimal Value
    {
        get => _value.RoundMoney();
        set => _value = value;
    }

    public decimal GetExactValue() => _value;

    public static implicit operator decimal(Money number) => number._value;
    public static implicit operator DecNumber(Money number) => number._value;
    public static implicit operator Money(decimal value) => new(value);

    public bool IsEqual(decimal value, decimal tolerance = 0.001m) => _value.IsEqual(value, tolerance);

    public decimal Round(int? roundDecimals = null, int extraPrecision = 0, int minPrecision = 0) => _value.Round(roundDecimals, extraPrecision, minPrecision);

    /// <summary>
    /// Applies percentage to the price values
    /// <code>
    /// price = 100;
    /// price.ApplyPercent(+5.Percent());   // 105
    /// price.ApplyPercent(-5.Percent());   // 95
    /// </code>
    /// </summary>
    public decimal ApplyPercent(decimal pct)
    {
        return (_value * (1 + pct / 100m)).RoundMoney();
    }

    public decimal Pow(double pow) => _value.Pow(pow);

    public decimal Sqrt() => _value.Sqrt();

    public decimal Abs() => _value.Abs();

    //public decimal Pct(decimal mean) => _value.Pct(mean);
    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => Value.ToString("C2",CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Value.ToString(format, formatProvider);
    }

    public string ToString(string format) => Value.ToString(format);

    public string ToStringExactValue(string format) => _value.ToString(format);

    public static decimal operator *(Money a, Money b) => (a._value * b._value);
    public static Money operator *(Money a, decimal b) => new(a._value * b);
    public static Money operator *(Money a, int b) => new(a._value * b);
    public static Money operator *(decimal a, Money b) => a * b._value;

    public static decimal operator /(Money a, Money b) => (a._value / b._value);    
    public static Money operator /(Money a, decimal b) => new(a._value / b);
    public static Money operator /(Money a, int b) => new(a._value / b);
    public static decimal operator /(decimal a, Money b) => a / b._value;

    public static Money operator +(Money a, Money b) => new(a._value + b._value);
    public static Money operator +(Money a, decimal b) => new(a._value + b);
    public static decimal operator +(decimal a, Money b) => a + b._value;

    public static Money operator -(Money a, Money b) => new(a._value - b._value);
    public static Money operator -(Money a, decimal b) => new(a._value - b);
    public static decimal operator -(decimal a, Money b) => a - b._value;

    public decimal SafeDivide(decimal divider) => _value.SafeDivide(divider);
    public Money SafeDivide(Money divider) => new(_value.SafeDivide(divider._value));

    public int CompareTo(decimal other)
    {
        return _value.CompareTo(other);
    }

    public int CompareTo(Money other)
    {
        return _value.CompareTo(other._value);
    }

    public int CompareTo(DecNumber other)
    {
        return _value.CompareTo(other.GetExactValue());
    }
}


public class MoneyJsonConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Money(reader.GetDecimal());
    }

    public override void Write(Utf8JsonWriter writer, Money number, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(number.Value);
    }
}
