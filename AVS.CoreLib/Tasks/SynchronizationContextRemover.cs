using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AVS.CoreLib.Tasks
{
    /// <summary>
    /// The SynchronizationContextRemover
    /// in OnCompleted method switches the synchronization context to null prior to the continuations being called
    /// so the other method calls will also use the null synchronization context
    /// therefore code is not marshalled back to the original synchronization context (i.e. the UI thread)
    /// </summary>
    /// <code>
    /// await new SynchronizationContextRemover();
    /// await RunAsync();
    /// await ProcessAsync();
    /// in pseudo code it will be:
    /// SynchronizationContextRemover.OnComplete(
    ///     RunAsync.OnComplete(
    ///         ProcessAsync().OnComplete()
    ///     )
    /// )
    /// </code>
    public struct SynchronizationContextRemover : INotifyCompletion
    {
        public bool IsCompleted => SynchronizationContext.Current == null;

        public void OnCompleted(Action continuation)
        {
            var prev = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                continuation();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prev);
            }
        }

        public SynchronizationContextRemover GetAwaiter()
        {
            return this;
        }

        public void GetResult()
        {
        }
    }
}