using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Structs;

/// <summary>
/// represents a wrapper for money and its percentage representation e.g. PNL: 100$ (2.5%)
/// the struct is srealized to array: [100, 0.25]
/// </summary>
[JsonConverter(typeof(MoneyPctJsonConverter))]
[DebuggerDisplay("{ToString()}")]
public readonly struct MoneyPct
{
    private readonly decimal _money; //holds $ value
    private readonly decimal _percent; // holds fraction i.e. 0.25 => 25%

    public MoneyPct(decimal value, decimal percent)
    {
        _money = value;
        _percent = percent;
    }

    public Money Money
    {
        get => _money.Round();
    }

    public Percent Percent => new(_percent);

    /// <summary>
    /// returns an exact fraction value e.g. 0.2525218 (stored internally) => 0.2525218
    /// </summary>
    /// <returns></returns>
    public decimal GetExactValue() => _money;

    // mutable structs are classic gotchas in .NET, if you need it mutable, most likely you need a class not struct!
    //public void Update(Money money, Percent percent)
    //{
    //    _money = money.GetExactValue();
    //    _percent = percent.GetExactValue();
    //}

    public override string ToString()
    {
        return _money == 0 ? "0" : $"{_money:C2} ({_percent:P2})";
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (format != null && format.StartsWith("P"))
            return Percent.ToString(format, formatProvider);

        if (format != null && format.StartsWith("C"))
            return Money.ToString(format, formatProvider);

        return _money == 0 ? "0" : $"{_money:C2} ({_percent:P2})";
    }

    public static implicit operator Money(MoneyPct obj) => obj.Money;
    public static implicit operator Percent(MoneyPct obj) => obj.Percent;


    public static bool operator >(MoneyPct a, decimal b) => a._money > b;
    public static bool operator <(MoneyPct a, decimal b) => a._money < b;
    public static bool operator >=(MoneyPct a, decimal b) => a._money >= b;
    public static bool operator <=(MoneyPct a, decimal b) => a._money <= b;

    public static bool operator >(MoneyPct a, MoneyPct b) => a._money > b._money;
    public static bool operator <(MoneyPct a, MoneyPct b) => a._money < b._money;
    public static bool operator >=(MoneyPct a, MoneyPct b) => a._money >= b._money;
    public static bool operator <=(MoneyPct a, MoneyPct b) => a._money <= b._money;

    //public static bool operator >(MoneyPct a, int b) => a.Percent > b;
    //public static bool operator <(MoneyPct a, int b) => a.Percent < b;
    //public static bool operator >=(MoneyPct a, int b) => a.Percent >= b;
    //public static bool operator <=(MoneyPct a, int b) => a.Percent <= b;
}

public class MoneyPctJsonConverter : JsonConverter<MoneyPct>
{
    public override MoneyPct Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
        //if (reader.TokenType == JsonTokenType.String)
        //{
        //    var str = reader.GetString() ?? string.Empty;

        //    // 1% means 1% so its fraction representation will be 0.01m
        //    if (str.EndsWith("%") && decimal.TryParse(str.TrimEnd('%'), out var value))
        //        return new Percent(value / 100);

        //    // otherwise treat as a normal fraction representation where 1 => 100% 
        //    return new Percent(decimal.Parse(str));
        //}

        //return new Percent(reader.GetDecimal());
    }

    public override void Write(Utf8JsonWriter writer, MoneyPct obj, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(obj.Money.Value);
        writer.WriteNumberValue(obj.Percent.AsWhole());
        //[11, 1.25] means 11$, 1.25%
        writer.WriteEndArray();
    }
}