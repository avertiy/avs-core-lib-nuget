using System;
using System.Collections.Generic;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.DLinq;

public class LexemeParser
{
    private static bool IsAny(string expr)
    {
        return expr == "*" || expr == ".*";
    }

    public Lexeme[] ParseExpression(string? selectExpression)
    {
        if (string.IsNullOrEmpty(selectExpression) || IsAny(selectExpression))
            return Array.Empty<Lexeme>();

        var parts = selectExpression.Split(',');
        var list = new List<Lexeme>(parts.Length);

        foreach (var part in parts)
        {
            var lexeme = Parse(part);
            list.Add(lexeme);
        }
        
        return list.ToArray();
    }

    private Lexeme Parse(string input)
    {
        Guard.Against.NullOrEmpty(input);

        //valid inputs:
        //x.Close
        //close
        //x.props.atr
        //x.bar["SMA(21)"]
        //x.arr[5]
        //A.B - prop A with a child prop B
        //.x.B - prop x with a child prop B
        //["key1"].B

        // remove leading `.` or `x.`
        var str = input;
        if (input[0] == '.')
            str = input.Substring(1);
        else if(input.StartsWith("x."))
            str = input.Substring(2);


        var ind = str.IndexOf('[');
        var dotIndex = str.IndexOf('.');

        if (dotIndex > -1 && (ind == -1 || ind > dotIndex))
            return ParseDotExpression(str, dotIndex);

        if (ind > -1)
            return ParseSquareBracketsExpression(str, ind);

        return new Lexeme(str);
    }

    private Lexeme ParseDotExpression(string input, int index)
    {
        //valid cases: props.atr, item.Prop["key"]
        //throw new NotImplementedException("dot expressions i.e. nested properties not supported yet");

        var prop = input.Substring(0, index);
        var result = new Lexeme(prop) { Raw = input };

        var str = input.Substring(index + 1);
        result.Inner = Parse(str);
        return result;
    }

    private Lexeme ParseSquareBracketsExpression(string input, int startIndex)
    {
        //valid cases: [5], ["key1"], bag["key1"], bag["key1"][3], bar["key1"].Value
        //invalid: [["key1"]]
        var endIndex = input.IndexOf(']', startIndex);

        if (endIndex < startIndex)
            throw new ArgumentException($"Invalid expression `{input}` - closing square bracket `]` is missing");

        var length = endIndex - startIndex - 1;

        if (length == 0)
            throw new ArgumentException($"Invalid expression `{input}` - index is missing");


        var prop = input.Substring(0, startIndex);
        var result = new Lexeme(prop) { Raw = input };

        var str = input.Substring(startIndex + 1, length);

        if (int.TryParse(str, out var index))
        {
            result.Index = index;
        }
        else if (str.StartsWith('"') && str.Length > 2 && str.EndsWith('"'))
        {
            result.Key = str.Substring(1, str.Length - 2);
        }
        else
        {
            throw new ArgumentException($"Invalid expression `{input}` - key should be wrapped in quotes [\"key\"], index should be an integer value");
        }

        if (endIndex < input.Length - 1)
        {
            var rest = input.Substring(endIndex + 1);
            result.Inner = Parse(rest);
        }

        return result;
    }

}