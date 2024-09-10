using System.Collections;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Enums;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class ObjectExtensions
{
    public static NumberFlags GetNumberFlags(this object val, string formattedValue)
    {
        var flags = NumberFlags.None;
        var sign = val.GetSign();
        if (sign < 0)
            flags = flags | NumberFlags.Negative;
        else if (sign == 0)
            flags = flags | NumberFlags.Zero;

        if (formattedValue.Contains('%'))
            flags = flags | NumberFlags.Percentage;
        if (formattedValue.ContainsAny("$", "USD", "EUR", "UAH"))
            flags = flags | NumberFlags.Currency;
        return flags;
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
            int or long or short => ObjType.Number,
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