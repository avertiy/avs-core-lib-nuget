using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.REST.Responses;
using AVS.CoreLib.REST.Utilities;

namespace AVS.CoreLib.REST.Extensions
{
    public static class TaskRunnerExtensions
    {
        [Obsolete("use ParallelExtensions util or TaskSet")]
        public static Dictionary<T, Task<IResponse<TResult>>> ForEach<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<Response<TResult>>> func)
        {
            var tasks = new Dictionary<T, Task<IResponse<TResult>>>();
            foreach (var key in enumerable)
                tasks.Add(key, SafeCall.Execute(async () => await func(key)));
            return tasks;
        }

        public static async Task<Dictionary<T, TResult>> ParallelFetch<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<Response<TResult>>> fetchFn, bool throwOnError = true)
        {
            var tasks = new Dictionary<T, Task<Response<TResult>>>();
            foreach (var key in enumerable)
            {
                var task = fetchFn(key);
                tasks.Add(key, fetchFn(key));
            }

            await Task.WhenAll(tasks.Values);

            var results = new Dictionary<T, TResult>();

            foreach (var kp in tasks)
            {
                var task = kp.Value;
                if (!task.Result.Success && throwOnError)
                {
                    task.Result.ThrowOnError();
                }

                results.Add(kp.Key, task.Result.Data);
            } 

            return results;
        }

        public static async Task<List<TItem>> ParallelFetch<T, TResult,TItem>(this IEnumerable<T> enumerable, 
            Func<T, Task<Response<TResult>>> fetchFn,
            Func<TResult,IEnumerable<TItem>> selector,
            int delay = 500,
            bool throwOnError = true)
        {
            var tasks = new Dictionary<T, Task<Response<TResult>>>();
            foreach (var key in enumerable)
            {
                var task = fetchFn(key);
                tasks.Add(key, fetchFn(key));
                Thread.Sleep(delay);
            }

            await Task.WhenAll(tasks.Values);

            var failedTasks = tasks.Count(x => !x.Value.Result.Success);

            if (failedTasks > 0 && failedTasks == tasks.Count && throwOnError)
            {
                tasks.First().Value.Result.ThrowOnError();
            }else if (failedTasks > 0 && failedTasks != tasks.Count)
            {
                var keys = tasks.Where(x => x.Value.Result.Success == false).Select(x => x.Key);

                foreach (var key in keys)
                {
                    var task = fetchFn(key);
                    tasks[key] = fetchFn(key);
                    Thread.Sleep(delay);
                }

                await Task.WhenAll(tasks.Values.Where(x => x.IsCompleted == false));
            }

            var results = new List<TItem>();

            foreach (var kp in tasks)
            {
                var task = kp.Value;
                if (!task.Result.Success && throwOnError)
                {
                    task.Result.ThrowOnError();
                }

                var items= selector(task.Result.Data);
                results.AddRange(items);
            }

            return results;
        }
    }
}