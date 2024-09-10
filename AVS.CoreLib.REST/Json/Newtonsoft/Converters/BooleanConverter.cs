using System;
using System.Runtime.CompilerServices;
using AVS.CoreLib.Enums;
using AVS.CoreLib.Extensions.Reflection;
using Newtonsoft.Json;

namespace AVS.CoreLib.REST.Json.Newtonsoft.Converters
{
    /// <summary>
    /// convert boolean value
    /// support <see cref="ValueModifier.Opposite"/> e.g. {"taker" : true} but you have a property: bool Maker { get; set;} = !Taker
    /// </summary>
    /// <example>
    /// [JsonConverter(typeof(BooleanConverter), ValueModifier.Opposite)]
    /// </example>
    public class BooleanConverter : JsonConverter
    {
        private readonly bool _hasOppositeModifier;
        public BooleanConverter(ValueModifier modifier = ValueModifier.None)
        {
            _hasOppositeModifier = modifier switch
            {
                ValueModifier.None => false,
                ValueModifier.Opposite => true,
                _ => throw new ArgumentOutOfRangeException(nameof(modifier), modifier, "No supported modifier")
            };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool) || objectType == typeof(bool?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || reader.Value == null)
            {
                var nullable = objectType.IsNullable();
                if (nullable)
                    return null;

                throw new JsonSerializationException($"Cannot convert null value to {objectType.Name}.");
            }

            bool value;
            switch (reader.Value.ToString().ToLower().Trim())
            {
                case "true":
                case "yes":
                case "y":
                case "1":
                    value = true;
                    break;
                case "false":
                case "no":
                case "n":
                case "0":
                    value = false;
                    break;
                default:
                    throw new JsonSerializationException($"Unable to parse boolean from `{reader.Value}`");
            }

            return ApplyModifiers(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;

            var bValue = (bool)value;
            writer.WriteValue(bValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ApplyModifiers(bool value)
        {
            if (_hasOppositeModifier)
                return !value;
            return value;
        }
    }
}