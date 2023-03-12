using System;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IFilter<T>
    {
        T Process(T obj);
    }
}