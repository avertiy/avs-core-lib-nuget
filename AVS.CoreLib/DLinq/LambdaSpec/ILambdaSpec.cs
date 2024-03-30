using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AVS.CoreLib.DLinq.LambdaSpec;

public interface ILambdaSpec
{
    string GetCacheKey<T>();
    Expression<Func<IEnumerable<T>, IEnumerable>> Build<T>();
}

[Flags]
public enum SpecMode
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