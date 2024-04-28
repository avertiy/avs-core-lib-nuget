using System;

namespace AVS.CoreLib.Extensions;

public enum ReductionFormat
{
    /// <summary>
    /// don't cut anything
    /// </summary>
    None = 0,
    /// <summary>
    /// truncate text 
    /// </summary>
    Truncated = 1,
    /// <summary>
    /// a bit less truncated than <see cref="Truncated"/> option
    /// </summary>
    Shortcut = 2,
}

//[Flags]
//public enum ExceptionView
//{
//    Default = 0,
//    /// <summary>
//    /// truncate text 
//    /// </summary>
//     = 1,
//    /// <summary>
//    /// a bit less truncated than <see cref="Truncated"/> option
//    /// </summary>
//    Shortcut = 2,
//}