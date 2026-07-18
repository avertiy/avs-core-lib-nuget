using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Structs;

/// <summary>
/// Represents a decimal wrapper to eliminate ambiguity when dealing with percentage values.
/// Percent representation is straight: 10 means 10%, 2.5 means 2.5% 
/// </summary>
[JsonConverter(typeof(PercentageJsonConverter))]
[DebuggerDisplay("Percentage {ToString()}")]
public struct Percentage : IComparable<decimal>, IComparable<Percentage>, IFormattable, IEquatable<Percentage>
{
    private decimal _value; // stored internally as percentage i.e. 10 means 10%

    public Percentage(decimal value)
    {
        _value = value;
    }

    /// <summary>
    /// Percentage value i.g. 10 means 10%
    /// </summary>
    public decimal Value
    {
        get => _value;
        set => _value = value;
    }

    [JsonIgnore]
    public decimal Percent => _value;

    [JsonIgnore]
    public decimal Pct => AsFraction();

    /// <summary>
    /// returns a fraction representation of the percentage e.g. 25% => 0.25
    /// </summary>
    public decimal AsFraction() => _value / 100;

    public Pct ToPct()
    {
        return new Pct(AsFraction());
    }

    public static explicit operator Percentage(decimal value)
    {
        return new Percentage(value);
    }

    public static implicit operator decimal(Percentage percentage)
    {
        return percentage.Value;
    }

    public bool IsEqual(decimal value, decimal tolerance) => _value.IsEqual(value, tolerance);

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => $"{Value}%";

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Value.ToString(format, formatProvider);
    }

    public string ToString(string format) => $"{Value.ToString(format)}";

    #region corelib methods
    public decimal Round(int? roundDecimals = null, int extraPrecision = 0, int minPrecision = 0) => _value.Round(roundDecimals, extraPrecision, minPrecision);

    public decimal Pow(double pow) => _value.Pow(pow);

    public decimal Sqrt() => _value.Sqrt();

    public decimal Abs() => _value.Abs();

    public decimal SafeDivide(decimal divider) => _value.SafeDivide(divider);
    public Percentage SafeDivide(Percentage divider) => new(_value.SafeDivide(divider._value));

    #endregion
    public int CompareTo(decimal other)
    {
        return AsFraction().CompareTo(other);
    }

    public int CompareTo(Percentage other)
    {
        return _value.CompareTo(other._value);
    }

    public static Percentage FromPercent(decimal value)
    {
        return new Percentage(value / 100m);
    }

    #region operators overload
    public static Percentage operator *(Percentage a, Percentage b) => new(a._value * b._value);
    public static Percentage operator /(Percentage a, Percentage b) => new(a._value / b._value);
    public static Percentage operator +(Percentage a, Percentage b) => new(a._value + b._value);
    public static Percentage operator -(Percentage a, Percentage b) => new(a._value - b._value);
    public static bool operator >(Percentage a, Percentage b) => a._value > b._value;
    public static bool operator >=(Percentage a, Percentage b) => a._value >= b._value;
    public static bool operator <(Percentage a, Percentage b) => a._value < b._value;
    public static bool operator <=(Percentage a, Percentage b) => a._value <= b._value;

    public static bool operator ==(Percentage a, Percentage b)
    {
        return a._value == b._value;
    }

    public static bool operator !=(Percentage a, Percentage b)
    {
        return !(a == b);
    }
    
    public static bool operator >(DecPct a, Percentage b) => a.Pct > b.AsFraction();
    public static bool operator >=(DecPct a, Percentage b) => a.Pct >= b.AsFraction();
    public static bool operator <(DecPct a, Percentage b) => a.Pct < b.AsFraction();
    public static bool operator <=(DecPct a, Percentage b) => a.Pct <= b.AsFraction();

    public static bool operator >(Percentage a, DecPct b) => a.AsFraction() > b.Pct;
    public static bool operator >=(Percentage a, DecPct b) => a.AsFraction() >= b.Pct;
    public static bool operator <(Percentage a, DecPct b) => a.AsFraction() < b.Pct;
    public static bool operator <=(Percentage a, DecPct b) => a.AsFraction() <= b.Pct;

    public static bool operator ==(Percentage a, DecPct b)
    {
        return a.AsFraction() == b.Pct;
    }

    public static bool operator !=(Percentage a, DecPct b)
    {
        return !(a == b);
    }

    public static bool operator ==(DecPct a, Percentage b)
    {
        return a.Pct == b.AsFraction();
    }

    public static bool operator !=(DecPct a, Percentage b)
    {
        return !(a == b);
    }

    #region operations with integers
    // int math => Percent result
    public static Percentage operator *(int a, Percentage b) => new(a * b._value);
    public static Percentage operator *(Percentage a, int b) => new(a._value * b);
    public static Percentage operator /(Percentage a, int b) => new(a._value / b);
    public static Percentage operator /(int a, Percentage b) => new(a / b._value);
    public static Percentage operator +(Percentage a, int b) => new(a._value + b);
    public static Percentage operator +(int a, Percentage b) => new(a + b._value);
    public static Percentage operator -(Percentage a, int b) => new(a._value - b);
    public static Percentage operator -(int a, Percentage b) => new(a - b._value);
    public static bool operator >(Percentage a, int b) => a._value > b;
    public static bool operator >=(Percentage a, int b) => a._value >= b;
    public static bool operator <(Percentage a, int b) => a._value < b;
    public static bool operator <=(Percentage a, int b) => a._value <= b; 
    #endregion
    
    #region decimals 

    // !!! decimal math as fraction => decimal result!!!
    public static decimal operator *(decimal a, Percentage b) => a * b.AsFraction();
    public static decimal operator *(Percentage a, decimal b) => a.AsFraction() * b;
    public static decimal operator /(Percentage a, decimal b) => a.AsFraction() / b;
    public static decimal operator /(decimal a, Percentage b) => a / b.AsFraction();
    public static decimal operator +(Percentage a, decimal b) => a.AsFraction() + b;
    public static decimal operator +(decimal a, Percentage b) => a + b.AsFraction();
    public static decimal operator -(Percentage a, decimal b) => a.AsFraction() - b;
    public static decimal operator -(decimal a, Percentage b) => a - b.AsFraction();
    public static bool operator >(Percentage a, decimal b) => a.AsFraction() > b;
    public static bool operator >=(Percentage a, decimal b) => a.AsFraction() >= b;
    public static bool operator <(Percentage a, decimal b) => a.AsFraction() < b;
    public static bool operator <=(Percentage a, decimal b) => a.AsFraction() <= b;
    public static bool operator >(decimal a, Percentage b) => a > b.AsFraction();
    public static bool operator >=(decimal a, Percentage b) => a >= b.AsFraction();
    public static bool operator <(decimal a, Percentage b) => a < b.AsFraction();
    public static bool operator <=(decimal a, Percentage b) => a <= b.AsFraction();

    #endregion

    #region DecPct

    public static decimal operator *(DecPct a, Percentage b) => a.Pct * b.Pct;
    public static decimal operator *(Percentage a, DecPct b) => a.Pct * b.Pct;
    public static decimal operator /(DecPct a, Percentage b) => a.Pct / b.Pct;
    public static decimal operator /(Percentage a, DecPct b) => a.Pct / b.Pct;
    public static decimal operator +(DecPct a, Percentage b) => a.Pct + b.Pct;
    public static decimal operator +(Percentage a, DecPct b) => a.Pct + b.Pct;
    public static decimal operator -(DecPct a, Percentage b) => a.Pct - b.Pct;
    public static decimal operator -(Percentage a, DecPct b) => a.Pct - b.Pct;

    #endregion

    #region DecNumber

    public static decimal operator *(DecNumber a, Percentage b) => a.Value * b.Pct;
    public static decimal operator *(Percentage a, DecNumber b) => a.Pct * b.Value;
    public static decimal operator /(DecNumber a, Percentage b) => a.Value / b.Pct;
    public static decimal operator /(Percentage a, DecNumber b) => a.Pct / b.Value;
    public static decimal operator +(DecNumber a, Percentage b) => a.Value + b.Pct;
    public static decimal operator +(Percentage a, DecNumber b) => a.Pct + b.Value;
    public static decimal operator -(DecNumber a, Percentage b) => a.Value - b.Pct;
    public static decimal operator -(Percentage a, DecNumber b) => a.Pct - b.Value;

    #endregion

    #endregion

    public bool Equals(Percentage other)
    {
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Percentage other && Equals(other);
    }
}

public class PercentageJsonConverter : JsonConverter<Percentage>
{
    public override Percentage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString() ?? string.Empty;

            // str is expected as a percentage value: "1" => 1%, "10" => 10% etc.

            if (str.EndsWith("%") && decimal.TryParse(str.TrimEnd('%'), out var value))
                return new Percentage(value);

            return new Percentage(decimal.Parse(str));
        }

        return new Percentage(reader.GetDecimal());
    }

    public override void Write(Utf8JsonWriter writer, Percentage percent, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(percent.Value);
    }
}


