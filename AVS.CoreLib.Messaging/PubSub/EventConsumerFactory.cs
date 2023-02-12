using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Abstractions.Messaging.PubSub;

namespace AVS.CoreLib.Messaging.PubSub
{
    /// <summary>
    /// resolve event consumer(s), if any registered in DI as <seealso cref="IEventConsumer{TEvent,TContext}"/>" 
    /// </summary>
    internal sealed class EventConsumerFactory : IEventConsumerFactory
    {
        private readonly Dictionary<Type, IEventConsumer[]> _subscribers = new Dictionary<Type, IEventConsumer[]>();
        private readonly ServiceFactoryResolveAll _serviceFactory;
        private readonly object _locker = new object();
        public EventConsumerFactory(ServiceFactoryResolveAll serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        /// <summary>
        /// Resolve event consumer(s), if any registered in DI as <seealso cref="IEventConsumer{TEvent,TContext}"/>" implementations 
        /// </summary>
        /// <param name="type">Type of consumer represented by a generic <see cref="IEventConsumer{TEvent,TContext}"/>"</param>
        public IEventConsumer[] ResolveAll(Type type)
        {
            if (_subscribers.ContainsKey(type))
            {
                return _subscribers[type];
            }

            lock (_locker)
            {
                if (_subscribers.ContainsKey(type))
                {
                    return _subscribers[type];
                }

                var enumerable = _serviceFactory(type);
                var consumers = enumerable.OfType<IEventConsumer>().ToArray();
                _subscribers.Add(type, consumers);
                return consumers;
            }
        }
    }
}