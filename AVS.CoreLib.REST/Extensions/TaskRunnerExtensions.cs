using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.REST.Responses;
using AVS.CoreLib.REST.Utilities;

namespace AVS.CoreLib.REST.Extensions
{
    public static class TaskRunnerExtensions
    {
        public static Dictionary<T, Task<IResponse<TResult>>> ForEach<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<Response<TResult>>> func)
        {
            var tasks = new Dictionary<T, Task<IResponse<TResult>>>();
            foreach (var key in enumerable)
                tasks.Add(key, SafeCall.Execute(async () => await func(key)));
            return tasks;
        }
    }
}