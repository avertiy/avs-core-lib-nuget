#nullable enable
using System;

namespace AVS.CoreLib.Messaging.Abstractions.PubSub
{
    public interface IPublishContext
    {
        PublishMode Mode { get; set; }
    }

    [Flags]
    public enum PublishMode
    {
        None = 0,
        /// <summary>
        /// when one of the consumer handle throws an exception, ignore the error and continue
        /// </summary>
        Isolated = 1,
        /// <summary>
        /// when no consumers found for the event throw an error
        /// </summary>
        Mandatory = 2,
        /// <summary>
        /// publish in async mode starting a new task for each consumer
        /// </summary>
        Parallel = 4,
        ParallelIsolated = Parallel | Isolated
    }
}