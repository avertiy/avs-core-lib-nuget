using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.DLinq.Specs.Conditioning;

[DebuggerDisplay("OperatorInSpec: IN({Args})")]
public class OperatorInSpec : ComparisonSpec
{
    public string[] Args { get; set; }
    public OperatorInSpec(params string[] args) : base(Operator.In) 
    {
        Args = args;
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var type = expression.Type;

        if (Args.Length == 1)
        {
            Arg = Args[0];
            return base.BuildExpr(expression, ctx);
        }

        var array = Parser.TryParse(Args, type);

        if (array == null)
            throw new SpecException($"Unable to parse `{string.Join(',', Args)}` as {type.GetReadableName()}[]", this);

        var arg = array.GetValue(0);
        var argExpr = Expression.Constant(arg);
        var leftExpr = Expression.Equal(expression, argExpr);

        for (var i = 1; i < array.Length; i++)
        {
            var arg2 = array.GetValue(i);
            var arg2Expr = Expression.Constant(arg2);
            var rightExpr = Expression.Equal(expression, arg2Expr);
            leftExpr = Expression.Or(leftExpr, rightExpr);
        }

        return leftExpr;
    }

    public override string ToString()
    {
        return $"IN ({string.Join(',', Args)})";
    }

    public static OperatorInSpec Parse(string str)
    {
        var args = str.TrimStart('(').TrimEnd(')').Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        return new OperatorInSpec(args);
    }
}