using System;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.Utilities;

/// <summary>
/// Synchronize output to console for multi-threading flows 
/// usage: 
/// <code>
///     using(var locker = ConsoleLocker.Create()){  console write operations..}
/// </code>
/// </summary>
public sealed class ConsoleLocker : IDisposable
{
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // 1 means it's essentially a mutex
    public bool LockTaken { get; private set; }
    public Guid Guid { get; set; }
    public int ThreadId { get; set; }
    public bool IsDebug { get; set; }

    // Dispose method to release the semaphore if it was acquired
    public void Dispose()
    {
        if (!LockTaken)
            return;

        _semaphore.Release();
        LockTaken = false; // Reset the flag
        if (IsDebug)
            Console.WriteLine($"Lock released ({Guid}, thread #{ThreadId})");
    }

    public static async Task<ConsoleLocker> CreateAsync(int millisecondsTimeout = 1000, bool debug = false)
    {
        var locker = new ConsoleLocker
        {
            Guid = Guid.NewGuid(),
            ThreadId = Thread.CurrentThread.ManagedThreadId
        };

        if (millisecondsTimeout <= 0)
            return locker;

        // Attempt to acquire the semaphore asynchronously
        locker.LockTaken = await _semaphore.WaitAsync(millisecondsTimeout);

        if (debug && locker.LockTaken)
            Console.WriteLine($"Lock acquired ({locker.Guid}, thread #{locker.ThreadId})");

        return locker;
    }

    public static ConsoleLocker Create(int millisecondsTimeout = 1000, bool debugMode = false)
    {
        var locker = new ConsoleLocker
        {
            Guid = Guid.NewGuid(),
            ThreadId = Thread.CurrentThread.ManagedThreadId,
            IsDebug = debugMode
        };

        if (millisecondsTimeout <= 0)
            return locker;

        // Try to acquire the semaphore within the specified timeout
        locker.LockTaken = _semaphore.Wait(millisecondsTimeout);

        if (locker.LockTaken)
            Console.WriteLine($"Lock acquired ({locker.Guid}, thread #{locker.ThreadId})");

        return locker;
    }

    public static async Task<TResult> Lock<TResult>(Func<Task<TResult>> func, int timeout = 1000, bool debug = false)
    {
        using (var locker = await CreateAsync(timeout, debug))
        {
            return await func();
        }
    }
}

//public sealed class ConsoleLocker : IDisposable
//{
//    private static readonly object _lock = new object();
//    private bool _lockWasTaken = false;
//    public Guid Guid { get; set; }
//    public int ThreadId { get; set; }
//    public static ConsoleLocker Create(int millisecondsTimeout = 1000)
//    {
//        var locker = new ConsoleLocker() { Guid = Guid.NewGuid() };
//        locker._lockWasTaken = Monitor.TryEnter(_lock, millisecondsTimeout);
//        //Monitor.Enter(_lock, ref locker._lockWasTaken);
//        locker.ThreadId = Thread.CurrentThread.ManagedThreadId;
//        return locker;
//    }

//    public void Dispose()
//    {
//        if (_lockWasTaken)
//        {
//            Monitor.Exit(_lock);
//            _lockWasTaken = false;
//        }
//    }
//}