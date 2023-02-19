using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVS.CoreLib.Extensions.Collections;

namespace AVS.CoreLib.Extensions.Tasks
{
    /// <summary>
    /// non generic utility to create <see cref="TaskSet{T,TResult}"/>
    /// </summary>
    public static class TaskSet
    {
        public static TaskSet<T, TResult> Create<T, TResult>(IEnumerable<T> enumerable, Func<T, Task<TResult>> func) where T : notnull
        {
            var taskSet = new TaskSet<T, TResult>(enumerable, func);
            return taskSet;
        }

        /// <summary>
        /// returns <see cref="Parallel{T, TResult}"/> wrapper
        /// </summary>
        public static TaskSet<T, TResult> CreateTaskSet<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<TResult>> job) where T : notnull
        {
            return new TaskSet<T, TResult>(enumerable, job);
        }
    }

    /// <summary>
    /// wrapper over <see cref="Dictionary{T, Task{TResult}}"/> typically when loading data for a large period, sometimes you send multiple requests i.e. initiating tasks
    /// and then combine results into list or whatever
    /// </summary>
    public class TaskSet<T, TResult> : IEnumerable<KeyValuePair<T, TResult>>
        where T : notnull
    {
        private readonly Dictionary<T, Task<TResult>> _tasks = new Dictionary<T, Task<TResult>>();

        public TaskSet(IEnumerable<T> enumerable, Func<T, Task<TResult>> job)
        {
            foreach (var arg in enumerable)
            {
                var task = job(arg);
                _tasks.Add(arg, task);
            }
        }

        public T[] Keys => _tasks.Keys.ToArray();
        public Task<TResult>[] Tasks => _tasks.Values.ToArray();

        public void Add(T arg, Task<TResult> task)
        {
            _tasks.Add(arg, task);
        }

        public Task WhenAll()
        {
            return Task.WhenAll(_tasks.Values);
        }
        
        public Dictionary<T, TResult> Results
        {
            get
            {
                var dict = new Dictionary<T, TResult>();
                foreach (var kp in _tasks)
                {
                    var task = kp.Value;
                    var result = task.IsCompleted ? task.Result : task.GetAwaiter().GetResult();
                    dict.Add(kp.Key, result);
                }
                return dict;
            }
        }

        public async Task<List<TResult>> ToListAsync()
        {
            await Task.WhenAll(_tasks.Values);
            var list = new List<TResult>();

            foreach (var kp in _tasks)
            {
                var task = kp.Value;
                var result = task.Result;
                list.Add(result);
            }

            return list;
        }


        public async Task<List<TItem>> ToListAsync<TItem>(Func<TResult, IEnumerable<TItem>> selector)
        {
            await Task.WhenAll(_tasks.Values);
            var list = new List<TItem>();

            foreach (var kp in _tasks)
            {
                var task = kp.Value;
                var result = task.Result;
                var items = selector(result);
                list.AddRange(items);
            }

            return list;
        }

        public IEnumerator<KeyValuePair<T, TResult>> GetEnumerator()
        {
            foreach (var kp in _tasks)
            {
                var task = kp.Value;
                var result = task.IsCompleted ? task.Result : task.GetAwaiter().GetResult();
                yield return new KeyValuePair<T, TResult>(kp.Key, result);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
