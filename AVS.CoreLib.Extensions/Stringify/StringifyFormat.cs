using System;

namespace AVS.CoreLib.Extensions.Stringify;

[Flags]
public enum StringifyFormat
{
    None = 0,
    Brackets = 1,
    Count = 2,
    MultiLine = 4,
    Limit = 8,
    Default = Brackets | Count | Limit
}