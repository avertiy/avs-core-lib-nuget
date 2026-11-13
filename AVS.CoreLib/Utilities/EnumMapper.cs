using System;
using System.Reflection;
using AVS.CoreLib.Attributes;
using AVS.CoreLib.Collections;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Utilities;

/// <summary>
/// EnumMapper provides generic Parse enum value from string, parse takes into account mappings <see cref="MapAttribute"/>
/// </summary>
public static class EnumMapper
{
    private static FixedDicitonary<string, object> _map = new(10, 100);// we don't cache all enum values, small cache up to 100 values should work fine

    public static T Parse<T>(string str) where T : struct, Enum
    {
        if (TryParse<T>(str, out var enumValue))
           return enumValue;

        throw new ArgumentException($"Unable to parse `{str}` to {typeof(T).FullName} enum value");
    }

    public static bool TryParse<T>(string str, out T value) where T : struct, Enum
    {
        if (_map.TryGetValue(str, out var obj))
        {
            if (obj is T enumValue)
            {
                value = enumValue;
                return true;
            }
                
        }

        if (TryGetMap(str, out value))
        {
            _map.Upsert(str, value);
            return true;
        }

        value = default;
        return false;
    }

    private static bool TryGetMap<T>(string str, out T enumValue)
    {
        var enumType = typeof(T);
        var enumMembers = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

        var normalizedStr = str.ReplaceAll(["_","-"," "], string.Empty);

        FieldInfo? memberMatched = null;

        foreach (var member in enumMembers)
        {
            if (member.Name.Equals(str, StringComparison.OrdinalIgnoreCase) ||
                member.Name.Equals(normalizedStr, StringComparison.OrdinalIgnoreCase))
            {
                enumValue = (T)member.GetValue(null)!;
                return true;
            }

            var map = member.GetCustomAttribute<MapAttribute>(false);

            if (map != null)
            {
                if (map.Contains(str) || map.Contains(normalizedStr)) //str equals to one of map values
                {
                    enumValue = (T)member.GetValue(null)!;
                    return true;
                }

                if (memberMatched == null && (map.Match(str) || map.MatchNormalized(normalizedStr)))
                {
                    // continue the loop due to match is die and it might overlap
                    // e.g. `STOP_LOSS` will match `STOP_LOSS_MARKET` while it could be a separate enum value
                    memberMatched = member;
                }
            }
        }

        if (memberMatched != null)
        {
            enumValue = (T)memberMatched.GetValue(null)!;
            return true;
        }

        foreach (var member in enumMembers)
        {
            if (str.Match(member.Name) || normalizedStr.Match(member.Name.Replace("_", "")))
            {
                enumValue = (T)member.GetValue(null)!;
                return true;
            }
        }

        if (int.TryParse(str, out var intValue))
        {
            if (Enum.IsDefined(enumType, intValue))
            {
                enumValue = (T)Enum.ToObject(enumType, intValue);
                return true;
            }
        }

        throw new ArgumentException($"Unable to parse `{str}` to {typeof(T).FullName} enum value");
    }
}
