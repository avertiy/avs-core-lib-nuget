using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace AVS.CoreLib.DLinq.Conditions;
/*
[DebuggerDisplay("{Raw}")]
public class Token : IEnumerable<(string token, Op op)>
{
    public string Raw { get; }
    public bool HasOROperator { get; }
    public bool HasANDOperator { get; }

    public Token(string raw)
    {
        Raw = raw.Trim();

        var ind = Raw.IndexOf(Condition.OR_TOKEN, StringComparison.OrdinalIgnoreCase);
        var ind2 = Raw.IndexOf(Condition.AND_TOKEN, StringComparison.OrdinalIgnoreCase);

        HasOROperator = ind > -1;
        HasANDOperator = ind2 > -1;
    }

    public IEnumerator<(string token, Op op)> GetEnumerator()
    {
        if (HasOROperator && HasANDOperator)
        {
            var parts = Raw.Split(Condition.OR_TOKEN, StringSplitOptions.RemoveEmptyEntries);
            for (var j = 0; j < parts.Length-1; j++)
            {
                var part = parts[j];
                var arr = part.Split(Condition.AND_TOKEN, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < arr.Length - 1; i++)
                {
                    var token = arr[i];
                    yield return (token.Trim(), Op.AND);
                }

                yield return (arr[^1].Trim(), Op.OR);
            }

            yield return (parts[^1].Trim(), Op.None);
        }
        else if (HasANDOperator)
        {
            var arr = Raw.Split(Condition.AND_TOKEN, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < arr.Length-1; i++)
            {
                var token = arr[i];
                yield return (token.Trim(), Op.AND);
            }

            yield return (arr[^1].Trim(), Op.None);

        }
        else if (HasOROperator)
        {
            var arr = Raw.Split(Condition.OR_TOKEN, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < arr.Length - 1; i++)
            {
                var token = arr[i];
                yield return (token.Trim(), Op.OR);
            }

            yield return (arr[^1].Trim(), Op.OR);
        }
        else
        {
            yield return (Raw, Op.None);
        }

    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
*/