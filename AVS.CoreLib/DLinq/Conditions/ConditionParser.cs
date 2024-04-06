using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVS.CoreLib.DLinq.Conditions;

public class ConditionParser
{
    public static ICondition Parse(string expression)
    {
        var input = expression.Trim(' ');
        if (string.IsNullOrEmpty(input))
            return Condition.Empty;

        var leftInd = input.IndexOf('(');
        var rightInd = input.IndexOf(')');

        if (leftInd == -1 && rightInd == -1)
        {
            return ParseSimpleExpression(input);
        }

        var sb = new StringBuilder(input);
        var stack = new Stack<int>();
        var counter = 0;
        var dict = new Dictionary<string, string>(6);
        var doubleQuote = false;
        var squareBracket = false;

        for (var i = 0; i < sb.Length; i++)
        {
            switch (sb[i])
            {
                case '[':
                {
                    squareBracket = true;
                    break;
                }
                case ']':
                {
                    squareBracket = false;
                    break;
                }
                case '"':
                {
                    doubleQuote = !doubleQuote;
                    break;
                }
                case '(' when doubleQuote == false && squareBracket == false:
                {
                    stack.Push(i);
                    break;
                }
                case ')' when doubleQuote == false && squareBracket == false:
                {
                    if (stack.Count == 0)
                        throw new InvalidExpression($"Opening bracket `(` is missing [before {i} position]", expression);

                    var left = stack.Pop();
                    var length = i - left + 1;
                    var str = sb.ToString(left, length);
                    var key = "@C" + counter++;
                    dict.Add(key, str.TrimStart('(').TrimEnd(')'));
                    sb.Replace(str, key, left, str.Length);
                    i = left + key.Length - 1;
                    break;
                }
            }
        }

        if (stack.Count > 0)
            throw new InvalidExpression($"Closing bracket `)` is missing [after {stack.Peek()} position]",expression);

        var cond = Process(sb.ToString(), dict);

        return cond;
    }

    /// <summary>
    /// simple expression implies expression without any brackets `(` or `)` 
    /// </summary>
    private static ICondition ParseSimpleExpression(string expr)
    {
        var parts = expr.SplitByOR();

        switch (parts.Length)
        {
            case 1:
            {
                var tokens = parts[0].SplitByAND();
                return Condition.AND(tokens);
            }
            default:
            {
                var conditions = parts.Select(x => Condition.AND(x.SplitByAND()));
                return Condition.Join(Op.OR, conditions);
            }
        }
    }

    private static ICondition Process(string expr, Dictionary<string, string> dict)
    {
        if (string.IsNullOrWhiteSpace(expr))
            return Condition.Empty;

        if (dict.ContainsKey(expr))
        {
            return Process(dict[expr], dict);
        }

        var tokens = expr.SplitByOR();

        switch (tokens.Length)
        {
            case 1:
            {
                var cond = GetCondition(tokens[0], dict);
                return cond;
            }
            default:
            {
                var conditions = tokens.Select(x => GetCondition(x, dict));
                return Condition.Join(Op.OR, conditions);
            }
        }
    }

    private static ICondition GetCondition(string expr, Dictionary<string, string> dict)
    {
        var parts = expr.SplitByAND();
        switch (parts.Length)
        {
            case 1:
            {
                return getCondition(parts[0].Trim());
            }
            default:
            {
                var conditions = parts.Select(x => getCondition(x.Trim()));
                return Condition.Join(Op.AND, conditions);
            }
        }

        ICondition getCondition(string part)
        {
            var cond = dict.ContainsKey(part)
                ? Process(dict[part], dict)
                : new Condition(part);
            return cond;
        }
    }
}
