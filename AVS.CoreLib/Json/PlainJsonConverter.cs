using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.Json;

/// <summary>
/// Converts an object or value to JSON, provides tools to limit serialization of large objects / structures
/// </summary>
public class PlainJsonConverter : JsonConverter<object?>
{
    public int MaxItemsCount { get; set; }
    public int MaxPropsCount { get; set; }
    public int DepthLimit { get; set; } = 2;

    /// <summary>
    /// in Brief mode when output buffer reaches BytesLimit (BytesPending+BytesCommitted) serialization will be forced to end.
    /// </summary>
    public int BytesLimit { get; set; }
    public SerializationMode Mode { get; set; }
    private bool BriefModeEnabled => Mode == SerializationMode.Brief;

    private int Depth { get; set; }

    public override bool CanConvert(Type typeToConvert)
    {
        return !typeToConvert.IsPrimitive && typeToConvert != typeof(decimal) && !typeToConvert.IsValueType;
    }

    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        Depth++;
        WriteInternal(writer, value, options);
        Depth--;
    }

    private void WriteInternal(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        if (BytesLimit > 0 && BriefModeEnabled && (writer.BytesPending + writer.BytesCommitted) > BytesLimit)
        {
            writer.WriteRawValue("...", skipInputValidation: true);
            return;
        }

        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        if (value is string str)
        {
            writer.WriteStringValue(str);
            return;
        }

        if (value is IEnumerable enumerable)
        {
            if (Depth > DepthLimit && DepthLimit > 0)
                writer.WriteRawValue(value.GetType().GetReadableName(), skipInputValidation: true);
            else
                WriteEnumerable(writer, enumerable, options);
            return;
        }

        WriteObj(writer, value, options);
    }

    private void WriteEnumerable(Utf8JsonWriter writer, IEnumerable enumerable, JsonSerializerOptions options)
    {
        switch (enumerable)
        {
            case IDictionary<string, string> strDict:
                WriteTypedDict(writer, strDict, writer.WriteString);
                break;
            case IDictionary<string, int> intDict:
                WriteTypedDict(writer, intDict, writer.WriteNumber);
                break;
            case IDictionary<string, decimal> decDict:
                WriteTypedDict(writer, decDict, writer.WriteNumber);
                break;
            case IDictionary<string, object> dict:
                WriteTypedDict(writer, dict, (prop, obj) =>
                {
                    writer.WritePropertyName(prop);
                    JsonSerializer.Serialize(writer, obj, options);
                });
                break;
            case IDictionary dict:
                WriteDict(writer, dict, (prop, obj) =>
                {
                    writer.WritePropertyName(prop);
                    JsonSerializer.Serialize(writer, obj, options);
                });
                break;
            case IEnumerable<string> strArr:
                WriteTypedArray(writer, strArr, writer.WriteStringValue);
                break;
            case IEnumerable<decimal> decArr:
                WriteTypedArray(writer, decArr, writer.WriteNumberValue);
                break;
            case IEnumerable<int> intArr:
                WriteTypedArray(writer, intArr, writer.WriteNumberValue);
                break;
            case IEnumerable<double> decArr:
                WriteTypedArray(writer, decArr, writer.WriteNumberValue);
                break;
            case IEnumerable<object> objArr:
                WriteTypedArray(writer, objArr, obj =>
                {
                    WriteJsonArrayValue(writer, obj, options);
                });
                break;
            default:
                WriteArray(writer, enumerable, options);
                break;
        }
    }

    #region Write JSON Array
    private void WriteTypedArray<T>(Utf8JsonWriter writer, IEnumerable<T> arr, Action<T> write)
    {
        var counter = 0;
        writer.WriteStartArray();
        foreach (var item in arr)
        {
            counter++;

            if (BriefModeEnabled && (MaxItemsCount > 0 && counter > MaxItemsCount || BytesLimit > 0 &&  (writer.BytesCommitted + writer.BytesPending) > BytesLimit))
            {
                // [ "item1", /*...*/ ]
                writer.WriteCommentValue("...");
                break;
            }

            write(item);
        }

        writer.WriteEndArray();
    }

    private void WriteArray(Utf8JsonWriter writer, IEnumerable enumerable, JsonSerializerOptions options)
    {
        var counter = 0;
        writer.WriteStartArray();

        foreach (var item in enumerable)
        {
            counter++;
            if (BriefModeEnabled && (MaxItemsCount > 0 && counter > MaxItemsCount || BytesLimit > 0 && (writer.BytesCommitted + writer.BytesPending) > BytesLimit))
            {
                // [ "item1", /*...*/ ]
                writer.WriteCommentValue("...");
                break;
            }

            WriteJsonArrayValue(writer, item, options);
        }

        writer.WriteEndArray();
    }

    private void WriteJsonArrayValue(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        switch (value)
        {
            case string str:
                writer.WriteStringValue(str);
                break;
            case int i:
                writer.WriteNumberValue(i);
                break;
            case decimal dec:
                writer.WriteNumberValue(dec);
                break;
            case long l:
                writer.WriteNumberValue(l);
                break;
            case double d:
                writer.WriteNumberValue(d);
                break;
            case DateTime dt:
                writer.WriteStringValue(dt);
                break;
            default:
                JsonSerializer.Serialize(writer, value, options);
                break;
        }
    }
    #endregion

    #region Write Dict
    private void WriteDict(Utf8JsonWriter writer, IDictionary dict,  Action<string, object?> write)
    {
        var counter = 0;
        writer.WriteStartObject();

        foreach (DictionaryEntry entry in dict)
        {
            counter++;
            if (BriefModeEnabled && (MaxItemsCount > 0 && counter > MaxItemsCount || BytesLimit > 0 && (writer.BytesCommitted + writer.BytesPending) > BytesLimit))
            {
                // [ "item1", /*...*/ ]
                writer.WriteCommentValue("...");
                break;
            }

            writer.WriteStartObject();
            var propertyName = entry.Key.ToString() ?? string.Empty;
            write(propertyName, entry.Value);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }

    private void WriteTypedDict<TKey, TValue>(Utf8JsonWriter writer, IDictionary<TKey, TValue> dict, Action<string, TValue> write)
    {
        var counter = 0;
        writer.WriteStartObject();

        foreach (var kp in dict)
        {
            counter++;

            if (BriefModeEnabled && (MaxItemsCount > 0 && counter > MaxItemsCount || BytesLimit > 0 && (writer.BytesCommitted + writer.BytesPending) > BytesLimit))
            {
                // [ "item1", /*...*/ ]
                writer.WriteCommentValue("...");
                break;
            }

            writer.WriteStartObject();
            var propertyName = kp.Key?.ToString() ?? string.Empty;
            write(propertyName, kp.Value);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    } 
    #endregion
    
    private void WriteObj(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var type = value.GetType();

        if (type.IsPrimitive || type == typeof(decimal) || type.IsValueType)
        {
            JsonSerializer.Serialize(writer, value, type, options);
            return;
        }

        writer.WriteStartObject();
        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead).ToArray();
        var counter = 0;
        foreach (var prop in props)
        {
            if (ShouldIgnore(prop, value))
                continue;

            var propertyNameAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
            var propertyName = ApplyPropertyNamingPolicy(propertyNameAttr?.Name ?? prop.Name, options);
            var val = prop.GetValue(value);

            if (val == null)
            {
                writer.WriteNull(propertyName);
                continue;
            }

            counter++;

            if (BriefModeEnabled && (MaxPropsCount > 0 && counter > MaxPropsCount || BytesLimit > 0 && (writer.BytesCommitted + writer.BytesPending) > BytesLimit))
            {
                // [ "item1", /*...*/ ]
                writer.WriteCommentValue("...");
                break;
            }

            if (val is string str)
            {
                writer.WriteString(propertyName, str);
                continue;
            }

            writer.WritePropertyName(propertyName);
            JsonSerializer.Serialize(writer, val, prop.PropertyType, options);
        }

        writer.WriteEndObject();
    }

    private string ApplyPropertyNamingPolicy(string propertyName, JsonSerializerOptions options)
    {
        return options.PropertyNamingPolicy != null ? options.PropertyNamingPolicy.ConvertName(propertyName) : propertyName;
    }

    private bool ShouldIgnore(PropertyInfo prop, object value)
    {
        var ignoreAttr = prop.GetCustomAttribute<JsonIgnoreAttribute>();
        if (ignoreAttr != null && ignoreAttr.Condition != JsonIgnoreCondition.Never)
        {
            if (ignoreAttr.Condition == JsonIgnoreCondition.Always)
                return true;

            var condition = ignoreAttr.Condition;
            var propValue = prop.GetValue(value);

            // Skip writing if [JsonIgnore] is present
            if ((condition == JsonIgnoreCondition.WhenWritingNull && (propValue == null)) ||
                (condition == JsonIgnoreCondition.WhenWritingDefault && IsDefaultValue(propValue, prop.PropertyType)))
            {
                return true;
            }
        }

        return false;
    }

    // Helper function to check if a value is the default for its type
    private static bool IsDefaultValue(object? value, Type type)
    {
        if (value == null)
            return true; // Reference types: default is null

        // For value types, check if the value equals its default
        var defaultValue = Activator.CreateInstance(type); // Get default for value type
        return value.Equals(defaultValue);
    }
}

public enum SerializationMode
{
    /// <summary>
    /// In Brief mode only first element of a collection/dictionary will be serialized
    /// This allows to get json shape omitting details in case of large data objects
    /// </summary>
    Brief = 0,
    Full = 1,
}