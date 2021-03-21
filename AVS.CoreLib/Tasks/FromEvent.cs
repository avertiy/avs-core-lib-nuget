using System;
using System.Threading.Tasks;

namespace AVS.CoreLib.Tasks
{
    /// <summary>
    /// util that allows to await till event is fired
    /// i.e. when we have async/await method and continuation part should be done after the event is fired 
    /// </summary>
    /// <remarks>
    /// It is inspired by Observable.FromEventPattern
    /// and works similar just avoiding overhead of Observable library
    /// </remarks>
    /// <example> 
    /// Task task = FromEvent.CreateTask(h => btn.Click += h, h => btn.Click -= h);
    /// await till event is triggered (button clicked)
    /// await task;
    /// </example>
    public static class FromEvent
    {
        public static async Task CreateTask(Action<EventHandler> addHandler, Action<EventHandler> removeHandler)
        {
            var taskEventProducer = new TaskProducer<object>();
            addHandler(taskEventProducer.Handler);
            await taskEventProducer.Task;
            removeHandler(taskEventProducer.Handler);
        }

        public static async Task<T> CreateTask<T>(Action<EventHandler> addHandler, Action<EventHandler> removeHandler)
        {
            var taskEventProducer = new TaskProducer<T>();

            addHandler(taskEventProducer.Handler);
            var task = await taskEventProducer.Task;
            removeHandler(taskEventProducer.Handler);
            return task;
        }

        public static async Task<T> CreateTask<T>(Action<Action<T>> addHandler, Action<Action<T>> removeHandler)
        {
            var taskEventProducer = new TaskProducer<T>();
            addHandler(taskEventProducer.Handler);
            var task = await taskEventProducer.Task;
            removeHandler(taskEventProducer.Handler);
            return task;
        }

        class TaskProducer<T>
        {
            readonly TaskCompletionSource<T> _tcs = new TaskCompletionSource<T>();

            public void Handler(object sender, EventArgs args)
            {
                _tcs.SetResult((T)sender);
            }

            public void Handler(T arg)
            {
                _tcs.SetResult(arg);
            }

            public Task<T> Task => _tcs.Task;
        }
    }
}