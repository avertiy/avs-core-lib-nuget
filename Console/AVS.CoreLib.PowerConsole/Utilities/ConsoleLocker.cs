using System;
using System.Threading;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    /// <summary>
    /// Synchronize output to console for multi-threading flows,
    /// remember and restore console color scheme <see cref="ColorScheme"/>
    /// usage: 
    /// <code>
    /// using(var locker = ConsoleLocker.Create()){  console write operations..}
    /// </code>
    /// </summary>
    public sealed class ConsoleLocker : IDisposable
    {
        private static readonly object _lock = new object();
        private bool _lockWasTaken = false;
        private readonly ColorScheme _previous;

        private ConsoleLocker()
        {
            _previous = ColorScheme.GetCurrentScheme();
        }

        public static ConsoleLocker Create()
        {
            var locker = new ConsoleLocker();
            Monitor.Enter(_lock, ref locker._lockWasTaken);
            return locker;
        }

        public static ConsoleLocker Create(ColorScheme scheme)
        {
            var locker = new ConsoleLocker();
            Monitor.Enter(_lock, ref locker._lockWasTaken);
            ColorScheme.ApplyScheme(scheme);
            return locker;
        }

        public void Dispose()
        {
            ColorScheme.ApplyScheme(_previous);
            if (_lockWasTaken)
                Monitor.Exit(_lock);
        }
    }
}