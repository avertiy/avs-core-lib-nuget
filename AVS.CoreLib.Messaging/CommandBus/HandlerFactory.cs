using System;
using System.Collections.Concurrent;

namespace AVS.CoreLib.Messaging.CommandBus
{
    abstract class HandlerFactory<TInterface, THandler>
        where TInterface : class
        where THandler : class
    {
        protected readonly ConcurrentDictionary<Type, THandler> Handlers = new ConcurrentDictionary<Type, THandler>();
        protected readonly ServiceFactory ServiceFactory;
        protected HandlerFactory(ServiceFactory serviceFactory)
        {
            ServiceFactory = serviceFactory;
        }

        public void Register(Type type, THandler handler)
        {
            var t = typeof(TInterface);
            if (type.GetInterface(t.Name) == null)
                throw new ArgumentException($"The type must implement {t.Name} interface");

            Handlers.AddOrUpdate(type, handler, (key, oldValue) => handler);
        }

        public virtual THandler Resolve(TInterface obj)
        {
            var type = obj.GetType();
            if (Handlers.TryGetValue(type, out THandler handler))
                return handler;
            return null;
            //throw new Exception($"{type.Name} was not registered");
        }

        /// <summary>
        /// Resolves THandler instance defined by genericType and interfaceType)
        /// </summary>
        protected THandler ResolveGeneric(Type genericType, Type interfaceType)
        {
            var handlerType = genericType.MakeGenericType(interfaceType);
            return ServiceFactory(handlerType) as THandler;
            //try
            //{
            //    var handler = ServiceFactory(handlerType) as THandler;
            //    if (handler == null)
            //        throw new Exception($"{interfaceType.Name} is not registered in DI container");
            //    return handler;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception($"ResolveGeneric for {interfaceType.Name} failed", ex);
            //}
        }

    }
}