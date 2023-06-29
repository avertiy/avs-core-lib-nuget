using System;

namespace AVS.CoreLib.Extensions.Enums;

public static class EnumExtensions
{
    public static string ToUpperString<T>(this T value, string format = "G") where T : Enum
    {
        return value.ToString(format).ToUpper();
    }

    public static string ToLowerString<T>(this T value, string format = "G") where T : Enum
    {
        return value.ToString(format).ToUpper();
    }
}