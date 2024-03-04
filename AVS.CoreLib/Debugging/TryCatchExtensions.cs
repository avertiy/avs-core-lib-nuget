using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AVS.CoreLib.Debugging
{
    public static class TryCatchExtensions
    {
        [DebuggerStepThrough]
        public static TryContext<T, bool> Try<T>(this T obj, Action<T> action, string? label = null)
        {
            var state = new TryContext<T, bool>(obj, action, label);
            return state;
        }

        [DebuggerStepThrough]
        public static TryContext<T, TResult> Try<T, TResult>(this T obj, Func<T, TResult> func, string? label = null)
        {
            var state = new TryContext<T, TResult>(obj, func, label);
            return state;
        }

        [DebuggerStepThrough]
        public static TryContext<T, TResult> TryAsync<T, TResult>(this T obj, Func<T, Task<TResult>> func, string? label = null)
        {
            var state = new TryContext<T, TResult>(obj, func, label);
            return state;
        }
    }
}