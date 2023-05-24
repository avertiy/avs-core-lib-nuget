using System.Collections;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Enums;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class ObjectExtensions
{
    public static (ObjType, FormatFlags) GetTypeAndFlags(this object val, string formattedValue)
    {
        var objType = val.GetObjType();
        var flags = FormatFlags.None;
        if (objType == ObjType.String)
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
        else if (objType == ObjType.Object)
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

        return (objType, flags);
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

}