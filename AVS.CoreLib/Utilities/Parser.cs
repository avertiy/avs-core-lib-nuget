using System;

namespace AVS.CoreLib.Utilities;

public interface IParser
{
    Array? TryParse(string[] items, Type type);
    object? TryParse(string input, Type? type = null);
}

public static class Parser
{
    public static IParser Instance { get; set; } = new DefaultParser();
    public static object? TryParse(string input, Type? type = null)
    {
        return Instance.TryParse(input, type);
    }

    public static Array? TryParse(string[] input, Type type)
    {
        return Instance.TryParse(input, type);
    }
}