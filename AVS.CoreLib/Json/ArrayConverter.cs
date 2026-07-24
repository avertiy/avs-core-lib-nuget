using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Attributes;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.Json;

/// <summary>
/// Serializes object to compact array json structure:
/// <code>
///  new Order { Price = 2.5, Qty=100 } => [2.5, 100]
/// </code>
/// Deserializes json array into custom type: 
/// <code>
/// [0.123, 125] =>
/// {
///    [ArrayProperty(0)]
///    double Price;
///    [ArrayProperty(1)]
///    int Qty; 
/// }
/// </code>
/// usage: [JsonConverter(typeof(ArrayConverter))]
/// </summary>
public class ArrayConverter : JsonConverter<object>
{
    //protected bool SkipNullValues = false;

    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }


    public override void Write(Utf8JsonWriter writer, object obj, JsonSerializerOptions options)
    {
        //JsonIgnoreCondition.        
        
        writer.WriteStartArray();
        var type = obj.GetType();
        var props = type.GetProperties();
        var list = new SortedList<int, (PropertyInfo prop, bool Exclusive)>();
        foreach (var p in props)
        {
            if (p.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                continue;

            var attr = p.GetCustomAttribute<ArrayPropertyAttribute>();
            if (attr != null)
            {
                list.Add(attr.Index, (p, attr.Exclusive));
                continue;
            }

            var ind = list.Count+1;
            list.Add(ind, (p, false));
        }

        //drop index gaps e.g. -10 0 1 2 3 etc. the gap is -10;0
        var dict = new Dictionary<int, (PropertyInfo prop, bool Exclusive)>(list.Count);
        var i = 0;
        foreach (var kp in list)
        {
            dict.Add(i++, kp.Value);
        }

        var last = -1;

        foreach (var kp in dict)
        {
            var prop = kp.Value.prop;
            var index = kp.Key;

            if (index == last)
                continue;

            if (prop.GetIndexParameters().Length > 0)
            {
                if (kp.Value.Exclusive)
                    last++;
                continue;
            }

            if (!prop.ShouldSerialize(type, obj))
            {
                if (kp.Value.Exclusive)
                    last++;
                continue;
            }

            while (index != last + 1)
            {
                if (options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingNull && options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingDefault)
                    writer.WriteNullValue();
                last++;
            }

            last = index;

            var value = prop.GetValue(obj);

            if (value == null)
            {
                if (options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingNull && options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingDefault)
                    writer.WriteNullValue();
            }
            else
            {
                WriteValue(writer, value, options);
            }

            if (kp.Value.Exclusive)
                break;
        }

        writer.WriteEndArray();
    }

    private static void WriteValue(Utf8JsonWriter writer, object obj, JsonSerializerOptions options)
    {
        if (options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault && obj.IsDefault())
            return;

        var json = obj.ToJson(options);
        writer.WriteRawValue(json);

        //var converterAttribute = prop.GetJsonConverterAttribute();

        //if (converterAttribute != null)
        //{
        //    var json = obj.ToJson(options);
        //    writer.WriteRawValue(json);
        //}

        //if (obj is bool b)
        //    writer.WriteBooleanValue(b);

        //if (obj.IsNumeric())
        //    writer.WriteNumberValue((decimal)obj);
        //else if (prop.PropertyType.IsSimpleType())
        //    //writer.WriteRawValue()
        //else
        //    serializer.Serialize(writer, obj);
    }
}