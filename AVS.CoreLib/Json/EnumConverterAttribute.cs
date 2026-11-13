using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Json;

/// <summary>
/// Use EnumConverter aatribute when you need to override the default <see cref="EnumConverter{T}"/> or when strcit control over mapping enum values is needed
/// When  placed on property the <see cref="DedicatedEnumConverter{T}"/> will be use the provided options to map the enum values. 
/// 
/// <code>
/// 1) simple usage:
/// [EnumConverter("one_way_mode", "hedge_mode")] // enum values are resolved via Enum.GetValues(..); 
/// public PositionMode Mode { get; set; }
/// 
/// 2) safe approach i.e. you control enum values directly 
/// [EnumConverter("one_way_mode", "hedge_mode", PositionMode.OneWay, PositionMode.Hedge)]
/// public PositionMode Mode { get; set; }
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class EnumConverterAttribute : JsonConverterAttribute
{
    public string[]? EnumOptions { get; }
    public object[]? Args { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="EnumConverterAttribute"/> with the specified converter type.
    /// </summary>
    public EnumConverterAttribute(params string[] options)
    {
        EnumOptions = options;
    }

    public EnumConverterAttribute(params object[] args)
    {
        Args = args;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert)
    {
        var converterType = typeof(DedicatedEnumConverter<>).MakeGenericType(typeToConvert);
        var converter = (JsonConverter)Activator.CreateInstance(converterType, args:[EnumOptions ?? Args])!;
        return converter;
    }
}

/// <summary>
/// Dedicated enum converter is based on explicit options approach defined directly on a property via <see cref="EnumConverterAttribute"/>
/// </summary>
/// <typeparam name="T">Enum</typeparam>
public sealed class DedicatedEnumConverter<T> : JsonConverter<T>
        where T : struct, Enum
{
    private const string NONE = "None";
    private const string UNDEFINED = "Undefined";

    public string[] EnumOptions { get; }
    public T[] EnumValues { get; }

    public DedicatedEnumConverter(string[] options)
    {
        EnumOptions = options;
        EnumValues = Enum.GetValues<T>();

        if (options.Length == EnumValues.Length - 1)
        {
            if (EnumValues[0].ToString() == "None")
                EnumOptions = [string.Empty, .. options];
        }

        Guard.MustBe.Equal(EnumOptions.Length, EnumValues.Length);
    }

    public DedicatedEnumConverter(object[] args)
    {
        var options = new List<string>();
        var values = new List<T>();
        foreach (var arg in args)
        {
            if (arg is string s)
                options.Add(s);
            else
                values.Add((T)arg);
        }

        EnumOptions = options.ToArray();

        if (values.Count == 0)
        {
            EnumValues = Enum.GetValues<T>();
            if (options.Count == EnumValues.Length - 1)
            {
                if (EnumValues[0].ToString().Either(NONE, UNDEFINED)) // if missing value is None or Undefined
                    EnumOptions = [string.Empty, .. options];
            }
        }
        else
        {
            EnumValues = values.ToArray();
        }

        Guard.MustBe.Equal(EnumOptions.Length, EnumValues.Length);
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Expected string but got {reader.TokenType}.");

        var str = reader.GetString();

        if (string.IsNullOrEmpty(str))
            return default;

        var ind = EnumOptions.FindIndex(x => x == str);

        if (ind == -1)
            throw new JsonException($"Unable to parse '{str}' as {typeof(T).Name}.");

        return EnumValues[ind];
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}