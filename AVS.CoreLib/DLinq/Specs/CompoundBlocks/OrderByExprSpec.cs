using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using AVS.CoreLib.Extensions.Enums;

namespace AVS.CoreLib.DLinq.Specs.CompoundBlocks;

/// <summary>
/// Represent a lambda specification single field (value) selector to be used with OrderBy extensions
/// e.g. source.OrderBy(x => x.close, SortDirection) or more complex: source.OrderBy(x => x.prop[0]['key1'], SortDirection)
/// </summary>
public class OrderByExprSpec : SpecBase
{
    public Sort SortDirection { get; set; }
    public ValueExprSpec Field { get; set; }

    //public List<ValueExprSpec>? ThenByFields { get; set; }

    public OrderByExprSpec(ValueExprSpec field, Sort sortOrder)
    {
        Field = field;
        SortDirection = sortOrder;
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var expr = Field.BuildExpr(expression, ctx);



        return expr;
    }

    public override string ToString(string arg, SpecView view = SpecView.Default)
    {
        if (SortDirection == Sort.None)
            return string.Empty;

        var dir = SortDirection == Sort.Desc ? "Descending" : string.Empty;

        return $"OrderBy{dir}({Field.ToString(arg, view)})";
    }

    public static OrderByExprSpec Parse(string input, Type targetType, Sort sortDirection)
    {
        var parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries);

        var valueSpec = ValueExprSpec.Parse(parts[0], targetType);
        var spec = new OrderByExprSpec(valueSpec, sortDirection);

        return spec;
    }
}