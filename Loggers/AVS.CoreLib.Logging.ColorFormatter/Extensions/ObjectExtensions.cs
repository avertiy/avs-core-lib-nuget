using System.Collections;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Enums;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class ObjectExtensions
{
    public static (ObjType, FormatFlags) GetTypeAndFlags(this object val, string formattedValue)
    {
        var type = val.GetObjType();
        var flags = FormatFlags.None;
        if (type == ObjType.String)
        {
            if(formattedValue.Length <= 5)
                flags = FormatFlags.ShortString;
            else if (formattedValue.Length > 50)
                flags = FormatFlags.Text;

            if (formattedValue.StartsWith('{') && formattedValue.EndsWith('}'))
                flags = FormatFlags.CurlyBrackets;
            else if (formattedValue.StartsWith('[') && formattedValue.EndsWith(']'))
                flags = FormatFlags.SquareBrackets;
            else if (formattedValue.StartsWith('(') && formattedValue.EndsWith(')'))
                flags = FormatFlags.Brackets;

            if (formattedValue.ContainsAll('{', '}',':','"'))
                flags = FormatFlags.Json;
            if (formattedValue.Length < 50 && formattedValue.Contains('%'))
                flags = FormatFlags.Percentage;
        }
        else if (type == ObjType.Object)
        {
            if (formattedValue.StartsWith('{') && formattedValue.EndsWith('}'))
                flags = FormatFlags.Json & FormatFlags.CurlyBrackets;
            else if (formattedValue.StartsWith('[') && formattedValue.EndsWith(']'))
                flags = FormatFlags.SquareBrackets;
        }

        if (val.IsNumeric())
        {
            var sign = val.GetSign();
            if (sign < 0)
                flags = flags | FormatFlags.Negative;
            else if (sign == 0)
                flags = flags | FormatFlags.Zero;

            if (formattedValue.Contains('%'))
                flags = flags | FormatFlags.Percentage;
            if (formattedValue.ContainsAny("$", "USD", "EUR", "UAH"))
                flags = flags | FormatFlags.Currency;
        }

        return (type, flags);
    }

    public static ObjType GetObjType(this object obj)
    {
        var objType = obj switch
        {
            null => ObjType.Null,
            bool => ObjType.Boolean,
            byte => ObjType.Byte,
            string => ObjType.String,
            double or decimal or float => ObjType.Float,
            int or long or short => ObjType.Integer,
            Enum => ObjType.Enum,
            TimeSpan => ObjType.Time,
            DateTime => ObjType.DateTime,
            Array => ObjType.Array,
            IList => ObjType.List,
            IDictionary => ObjType.Dictionary,
            _ => ObjType.Object
        };

        return objType;
    }

    public static bool IsInteger(this object obj)
    {
        return obj is int or long or short;
    }

    public static bool IsFloating(this object obj)
    {
        return obj is double or decimal or float;
    }

    public static bool IsNumeric(this object obj)
    {
        return obj is int or long or short or double or decimal or float;
    }

    public static bool IsPositive(this object obj)
    {
        return obj switch
        {
            int i => i > 0,
            long l => l > 0,
            double d => d > 0,
            decimal dec => dec > 0,
            short s => s > 0,
            float f => f > 0,
            _ => false
        };
    }

    public static int GetSign(this object obj)
    {
        return obj switch
        {
            int i => i == 0 ? 0 : i > 0? 1: -1,
            long l => l == 0 ? 0 :l > 0 ? 1 : -1,
            double d => d == 0 ? 0 : d > 0 ? 1 : -1,
            decimal dec => dec == 0 ? 0 : dec > 0 ? 1 : -1,
            short s => s == 0 ? 0 : s > 0 ? 1 : -1,
            float f => f == 0 ? 0 : f > 0 ? 1 : -1,
            _ => 0
        };
    }
}