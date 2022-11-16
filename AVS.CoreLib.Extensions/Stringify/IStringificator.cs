using System;
using System.Collections.Generic;
using System.Text;

namespace AVS.CoreLib.Extensions.Stringify;

public interface IStringificator
{
    string Stringify<T>(IEnumerable<T> enumerable,
        StringifyOptions? options = null,
        Func<T, string>? formatter = null);

    string Stringify<TKey, TValue>(IDictionary<TKey,TValue> dictionary,
        StringifyOptions? options = null,
        Func<TKey, TValue, string>? formatter = null);
}


public sealed class Stringificator : IStringificator
{
    private static IStringificator? _instance;
    public static IStringificator Instance
    {
        get => _instance ??= new Stringificator();
        set => _instance = value;
    }

    public string Stringify<T>(IEnumerable<T> enumerable, StringifyOptions? options = null, Func<T, string>? formatter = null)
    {
        var format = StringifyFormat.Default;
        var separator = ",";
        var maxLength =0;
        if (options != null)
        {
            format = options.Format;
            separator = options.Separator;
            maxLength = options.MaxLength;
        }

        var brackets = format.HasFlag(StringifyFormat.Brackets);
        var displayCount = format.HasFlag(StringifyFormat.Count);
        var multiLine = format.HasFlag(StringifyFormat.MultiLine);
        var limit = format.HasFlag(StringifyFormat.Limit);
        var padding = " ";

        var sb = new StringBuilder();

        if (brackets)
            sb.Append('[');

        var count = 0;
        var l = "..".Length + separator.Length + (brackets ? 1 : 0);
        var reachedLimit = false;
        foreach (var item in enumerable)
        {
            if (reachedLimit)
            {
                // break if no need to count items  
                if (!displayCount)
                    break;

                count++;
                continue;
            }

            var str = formatter == null ? item?.ToString() : formatter(item);

            if (count == 0 && (multiLine || str?.Length > 10))
                multiLine = true;

            if (multiLine)
                sb.AppendLine();

            if (padding.Length > 0)
                sb.Append(padding);

            sb.Append(str);
            sb.Append(separator);

            if (maxLength > 0 && limit && (multiLine && count > 20 || sb.Length + l + padding.Length > maxLength))
            {
                if (multiLine)
                    sb.AppendLine();
                else
                    sb.Length = maxLength - l - padding.Length;

                if (padding.Length > 0)
                    sb.Append(padding);

                sb.Append("..");
                sb.Append(separator);
                reachedLimit = true;
            }
            count++;
        }

        if (count > 0 && separator.Length > 0)
            sb.Length -= separator.Length;

        if (multiLine)
            sb.AppendLine();

        if (brackets)
            sb.Append(']');

        if (displayCount && (count > 4 || reachedLimit))
            sb.Append($" (#{count})");

        return sb.ToString();
    }

    public string Stringify<TKey, TValue>(IDictionary<TKey, TValue> dictionary, StringifyOptions? options = null, Func<TKey, TValue, string>? formatter = null)
    {
        var format = StringifyFormat.Default;
        var separator = ",";
        var keyValueSeparator = ":";
        var maxLength = 0;
        if (options != null)
        {
            format = options.Format;
            separator = options.Separator;
            keyValueSeparator = options.KeyValueSeparator;
            maxLength = options.MaxLength;
        }

        var brackets = format.HasFlag(StringifyFormat.Brackets);
        var displayCount = format.HasFlag(StringifyFormat.Count);
        var multiLine = format.HasFlag(StringifyFormat.MultiLine);
        var limit = format.HasFlag(StringifyFormat.Limit);
        var padding = " ";
        var sb = new StringBuilder();
        if (brackets)
            sb.Append('[');

        var count = 0;
        var l = "..".Length + separator.Length + (brackets ? 1 : 0);
        var reachedLimit = false;
        foreach (var kp in dictionary)
        {
            if (reachedLimit)
            {
                // break if no need to count items  
                if (!displayCount)
                    break;

                count++;
                continue;
            }

            string? str = null;
            if (formatter == null)
            {
                var key = kp.Key?.ToString();
                var value = kp.Value?.ToString() ?? "null";
                str = key + keyValueSeparator + value;
            }
            else
                str = formatter(kp.Key, kp.Value);

            if (count == 0 && (multiLine || str.Length > 10))
                multiLine = true;

            if (multiLine)
                sb.AppendLine();

            if (padding.Length > 0)
                sb.Append(padding);

            sb.Append(str);
            sb.Append(separator);

            if (limit && (multiLine && count > 20 || sb.Length + l + padding.Length > maxLength))
            {
                if (multiLine)
                    sb.AppendLine();
                else
                    sb.Length = maxLength - l - padding.Length;

                if (padding.Length > 0)
                    sb.Append(padding);

                sb.Append("..");
                sb.Append(separator);
                reachedLimit = true;
            }

            count++;
        }

        if (count > 0 && separator.Length > 0)
            sb.Length -= separator.Length;

        if (multiLine)
            sb.AppendLine();

        if (brackets)
            sb.Append(']');

        if (displayCount && (count > 4 || reachedLimit))
            sb.Append($" (#{count})");

        return sb.ToString();
    }
}
