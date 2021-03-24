using AVS.CoreLib.EventSourcing.Events;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Abstractions.Messaging.PubSub;

namespace AVS.CoreLib.EventSourcing
{
    public abstract class EventSourcedAggregate
    {
        protected Queue<IEvent> PendingEvents { get; set; }

        protected EventSourcedAggregate()
        {
            PendingEvents = new Queue<IEvent>();
        }

        protected void Append(IEvent @event)
        {
            PendingEvents.Enqueue(@event);
        }

        protected abstract void LoadFromHistory(IEnumerable<IEvent> history);

        public IEnumerable<IEvent> GetUncommitedChanges()
        {
            return this.PendingEvents.AsEnumerable();
        }

        public void Commit()
        {
            this.PendingEvents.Clear();
        }
    }
}