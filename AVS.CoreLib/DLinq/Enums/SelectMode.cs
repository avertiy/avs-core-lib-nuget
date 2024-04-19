using System;

namespace AVS.CoreLib.DLinq.Enums;

[Flags]
public enum SelectMode
{
    /// <summary>
    /// return as is i.e. IEnumerable
    /// </summary>
    Default = 0,
    /// <summary>
    /// call ToList() 
    /// </summary>
    ToList = 1,
    /// <summary>
    /// value expressions will be wrapped into try catch blocks, in case of exception returns a default 
    /// </summary>
    Safe = 2,
    ToListSafe = ToList | Safe
}