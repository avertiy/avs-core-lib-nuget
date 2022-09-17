using System;

namespace AVS.CoreLib.Debugging
{
    public static class SyncUtil
    {
        private static readonly object _lock = new object();
        public static void RunSync(Action action)
        {
            lock (_lock)
            {
                action();
            }
        }

        public static T RunSync<T>(Func<T> fn)
        {
            lock (_lock)
            {
                return fn();
            }
        }
    }
}