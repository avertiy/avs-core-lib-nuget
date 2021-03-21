using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Abstractions.Messaging.PubSub;

namespace AVS.CoreLib.Messaging.PubSub
{
    internal sealed class EventConsumerFactory : IEventConsumerFactory
    {
        private readonly Dictionary<Type, IEventConsumer[]> _subscribers = new Dictionary<Type, IEventConsumer[]>();
        private readonly ServiceFactoryResolveAll _serviceFactory;
        private readonly object _locker = new object();
        public EventConsumerFactory(ServiceFactoryResolveAll serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }
       
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