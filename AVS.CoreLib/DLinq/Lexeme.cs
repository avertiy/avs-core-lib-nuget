using System;
using System.Reflection;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq;

public class Lexeme
{
    public string Property { get; set; }
    public string? Raw { get; set; }
    public int Index { get; set; } = -1;
    public string? Key { get; set; }
    public Lexeme? Inner { get; set; }
    public bool IsSimple => Inner == null && Key == null && Index == -1;

    public Lexeme(string property)
    {
        Property = property;
    }

    public ExpressionType GetExpressionType()
    {
        if (Inner != null)
            return ExpressionType.Compound;

        if (Key != null)
            return ExpressionType.Key;

        return Index > -1 ? ExpressionType.Index : ExpressionType.Default;
    }

    public override string ToString()
    {
        if (Raw != null)
            return Raw;

        string str;
        if (Key != null || Index > -1)
        {
            str = $"{Property}[{Key ?? Index.ToString()}]{Inner}";
        }
        else
        {
            str = $"{Property}{Inner}";
        }

        return str;
    }

    public string GetResultKey()
    {
        var key = Inner switch
        {
            null when Key == null && Index == -1 => Property,
            null => Key ?? $"{Property}[{Index}]",
            _ => Inner.GetResultKey()
        };

        return key.ToCamelCase();
    }

    public Type GetResultType(PropertyInfo prop)
    {
        switch (Inner)
        {
            case null when Key == null && Index == -1:
                return prop.PropertyType;
            case null when Key == null:
                return prop.PropertyType.GetIndexerRequired(typeof(int)).ReturnType;
            case null:
                return prop.PropertyType.GetIndexerRequired(typeof(string)).ReturnType;
            default:
            {
                var innerProp = GetInnerProperty(prop);
                return Inner.GetResultType(innerProp);
            }
        }
    }

    public PropertyInfo GetInnerProperty(PropertyInfo prop)
    {
        if (Inner == null)
            throw new InvalidOperationException("Inner property is null");

        var type = prop.PropertyType;

        if (Index > -1)
        {
            var methodInfo = type.GetIndexerRequired(typeof(int));
            type = methodInfo.ReturnType;
        }
        else if (Key != null)
        {
            var methodInfo = type.GetIndexerRequired(typeof(string));
            type = methodInfo.ReturnType;
        }

        var innerProp = type.GetProperty(Inner.Property,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (innerProp == null)
            throw new Exception($"Property {Inner.Property} not found in {type.Name} type definition");

        return innerProp;
    }


}

public enum ExpressionType
{
    Default = 0,
    Index =1,
    Key =2,
    Compound =3
}