using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Json;

public class PlainJsonConverter : JsonConverter<object?>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return true;
    }

    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        if (value == null)
            return;

        if (value is string str)
        {
            writer.WriteStringValue(str);
            return;
        }

        if (value is IDictionary<string, object> dict)
        {
            WriteDict(writer, dict);
            return;
        }

        if (value is IDictionary<string, decimal> decDict)
        {
            WriteDict(writer, decDict);
            return;
        }

        if (value is ICollection<object> list)
        {
            WriteList(writer, list);
            return;
        }

        WriteObj(writer, value);
    }

    private void WriteObj(Utf8JsonWriter writer, object value)
    {
        var type = value.GetType();
        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var sb = new StringBuilder(props.Length * 20);

        foreach (var prop in props)
        {
            if(!prop.CanRead)
                continue;

            var val = prop.GetValue(value);

            if (val == null)
                continue;

            if (prop.PropertyType.IsValueType)
            {
                var defaultValue = Activator.CreateInstance(prop.PropertyType);

                if (val.Equals(defaultValue))
                    continue;

                sb.Append($"{prop.Name.ToCamelCase()}:{FormatValue(val)}; ");
            }
            else
            {
                sb.Append($"{prop.Name.ToCamelCase()}:{val}; ");
            }   
        }

        if (sb.Length > 0)
            sb.Length-=2;

        var plainText = sb.ToString();
        writer.WriteStringValue(plainText);

    }

    private void WriteDict(Utf8JsonWriter writer, IDictionary<string, object> dict)
    {
        var sb = new StringBuilder(dict.Count * 20);
        foreach (var kp in dict)
        {
            sb.Append($"{kp.Key.ToCamelCase()}:{FormatValue(kp.Value)}; ");
        }

        if (sb.Length > 0)
            sb.Length-=2;

        var plainText = sb.ToString();
        writer.WriteStringValue(plainText);
    }

    private void WriteDict(Utf8JsonWriter writer, IDictionary<string, decimal> dict)
    {
        var sb = new StringBuilder(dict.Count * 20);
        foreach (var kp in dict)
        {
            sb.Append($"{kp.Key.ToCamelCase()}:{kp.Value.Round()}; ");
        }

        if (sb.Length > 0)
            sb.Length -= 2;

        var plainText = sb.ToString();
        writer.WriteStringValue(plainText);
    }

    private void WriteList(Utf8JsonWriter writer, ICollection<object> list)
    {
        var sb = new StringBuilder(list.Count * 10);
        foreach (var item in list)
        {
            sb.Append(FormatValue(item));
            sb.Append(", ");
        }

        if (sb.Length > 0)
            sb.Length -= 2;

        var plainText = sb.ToString();
        writer.WriteStringValue(plainText);
    }

    private string? FormatValue(object val)
    {
        if (val is decimal dec)
            return dec.Round().ToString(CultureInfo.InvariantCulture);

        if (val is double d)
            return d.Round().ToString(CultureInfo.InvariantCulture);

        if (val is DateTime dt)
            return dt.ToString("G", CultureInfo.InvariantCulture);

        return val.ToString();
    }
}