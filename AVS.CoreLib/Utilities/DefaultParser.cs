using System;
using System.Globalization;
using System.Linq;
using AVS.CoreLib.Dates;

namespace AVS.CoreLib.Utilities;

public class DefaultParser : IParser
{
    public object? TryParse(string input, Type? type = null)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (string.Equals(input, "NULL", StringComparison.OrdinalIgnoreCase))
            return null;

        if (type == null)
            return ParseInternal(input);

        return ParseForType(input, type);
    }

    protected object? ParseInternal(string input)
    {
        if ((input.IndexOf('.') > -1 || input.EndsWith('m')) && decimal.TryParse(input.TrimEnd('m'), out var dec))
            return dec;

        if (input.StartsWith('"') && input.EndsWith('"'))
            return input.Trim('"');

        if (input.StartsWith('\'') && input.EndsWith('\''))
            return input.Trim('\'');

        if (int.TryParse(input, out var i))
            return i;

        if (input.EndsWith('L') && long.TryParse(input.TrimEnd('L'), out var l))
            return l;

        if (input.EndsWith('b') && byte.TryParse(input.TrimEnd('b'), out var @byte))
            return @byte;

        if (bool.TryParse(input, out var b))
            return b;

        if (DateTime.TryParse(input, CultureInfo.InvariantCulture, out var dateTime))
            return dateTime;

        if (DateRange.TryParse(input, out var dateRange))
            return dateRange;

        return null;
    }

    private object? ParseForType(string input, Type type)
    {
        if (type.IsPrimitive)
            return ParsePrimitive(input, type);

        if (type.IsEnum)
            return Enum.Parse(type, input, true);

        if (type.IsValueType)
        {
            if (type == typeof(decimal))
                return decimal.TryParse(input, out var dec) ? (object?)dec : null;

            if (type == typeof(DateTime))
                return DateTime.TryParse(input, out var dateTime) ? dateTime : null;

            if (type == typeof(DateRange))
                return DateRange.TryParse(input, out var dateRange) ? (object?)dateRange : null;

            if (type == typeof(Guid))
                return Guid.TryParse(input, out var dateRange) ? (object?)dateRange : null;

            return null;
        }

        if (type.IsArray)
        {
            var items = input.TrimStart('[').TrimEnd(']').Split(',', StringSplitOptions.RemoveEmptyEntries);
            return TryParse(items, type.GetElementType()!);
        }

        if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var argType = type.GetGenericArguments().First();
                return ParseForType(input, argType);
            }
        }

        if (type.IsClass)
        {
            if (string.Equals(input, "NULL", StringComparison.OrdinalIgnoreCase))
                return true;

            if (type == typeof(string))
                return input;
        }

        return ParseInternal(input);
    }

    protected object? ParsePrimitive(string input, Type type)
    {
        if (type == typeof(int))
            return int.TryParse(input, out var i) ? i : null;

        if (type == typeof(double))
            return double.TryParse(input, out var d) ? d : null;

        if (type == typeof(long))
            return long.TryParse(input, out var i) ? i : null;

        if (type == typeof(bool))
            return bool.TryParse(input, out var res) ? res : null;

        if (type == typeof(byte))
            return byte.TryParse(input, out var res) ? res : null;

        if (type == typeof(short))
            return short.TryParse(input, out var res) ? res : null;

        return null;
    }

    public Array? TryParse(string[] items, Type type)
    {
        if (type == typeof(int))
            return items.Select(int.Parse).ToArray();

        if (type == typeof(decimal))
            return items.Select(decimal.Parse).ToArray();

        if (type == typeof(double))
            return items.Select(double.Parse).ToArray();

        if (type == typeof(long))
            return items.Select(long.Parse).ToArray();

        if (type == typeof(DateTime))
            return items.Select(x => DateTime.Parse(x, CultureInfo.InvariantCulture)).ToArray();

        if (type.IsEnum)
            return items.Select(x => Enum.Parse(type, x)).ToArray();

        return null;
    }

}