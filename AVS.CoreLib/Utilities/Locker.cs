using System;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.Utilities;

/// <summary>
/// Locker helps to sync operations for example logging to console in multi-threading/parallel-tasks scenarios
/// usage: 
/// <code>
///     using var locker = await Locker.CreateAsync();
///     Logger.Log("Starting operation");
///     await Operation();
///     Logger.Log("Finished operation"); 
/// </code>
/// </summary>
public sealed class Locker : IDisposable
{
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // 1 means it's essentially a mutex

    private static int _lockedByThreadId;
    
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
        _lockedByThreadId = 0;
        LockTaken = false; // Reset the flag
        if (IsDebug)
            Console.WriteLine($"Lock released ({Guid}, thread #{ThreadId})");
    }

    public static async Task<Locker> CreateAsync(int millisecondsTimeout = 1000, bool debug = false)
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;
        var locker = new Locker
        {
            Guid = Guid.NewGuid(),
            ThreadId = threadId
        };

        if (threadId == _lockedByThreadId)
            return locker;

        if (millisecondsTimeout <= 0)
            return locker;
        // Attempt to acquire the semaphore asynchronously
        locker.LockTaken = await _semaphore.WaitAsync(millisecondsTimeout);

        if (locker.LockTaken)
        {
            _lockedByThreadId = threadId;
            if (debug)
                Console.WriteLine($"Lock acquired ({locker.Guid}, thread #{locker.ThreadId})");
        }

        return locker;
    }

    public static Locker Create(int millisecondsTimeout = 1000, bool debugMode = false)
    {
        var locker = new Locker
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