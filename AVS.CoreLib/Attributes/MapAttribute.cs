using System;
using System.Linq;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Attributes;

/// <summary>
/// Map a enum entry to string values
/// <code>
/// // usage:
///  public enum MarginMode {
///     [Map("crossed", "cross")]
///     CrossMargin,
///     [Map("isolated", "fixed")]
///     IsolatedMargin
///  }
/// </code>
/// </summary>
public class MapAttribute : Attribute
{
    /// <summary>
    /// Values mapping to the enum entry
    /// </summary>
    public string[] Values { get; set; }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="maps"></param>
    public MapAttribute(params string[] maps)
    {
        Values = maps;
    }

    public bool Contains(string str, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return Values.Any(x => x.Equals(str, comparison));
    }

    public bool Match(string str)
    {
        return Values.Any(x => x.Match(str));
    }

    public bool MatchNormalized(string str)
    {
        foreach (var value in Values)
        {
            var s = value.ContainsAny('-', '_', ' ') 
                ? value.ReplaceAll(["-", "_", " "], "")
                : value;

            if (s.Match(str))
                return true;
        }

        return false;
    }
}