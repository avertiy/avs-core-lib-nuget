#nullable enable
using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Collections;

/// <summary>
/// Represents a wrapper over <see cref="ICollection{T}"/>, that allows to accumulate and filter data
/// </summary>
public interface IAccumulator<T> : IEnumerable<T>, IListWrapper<T>
{
    Func<T, bool>? Filter { get; set; }
    int AddRecords(IList<T> records);
    int Count { get; }
}