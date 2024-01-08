using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Messaging.Abstractions.CommandBus;

namespace AVS.CoreLib.Messaging.CommandBus
{
    /// <summary>
    /// registered in DI as a singleton
    /// </summary>
    sealed class CommandHandlerFactory : HandlerFactory<ICommand, ICommandHandler>, ICommandHandlerFactory
    {
        public CommandHandlerFactory(ServiceFactory serviceFactory) : base(serviceFactory)
        {
        }

        /// <summary>
        /// you don't need to call Register method explicitly, just register message handler in DI
        /// </summary>
        private void Register<TCommand>(ICommandHandler handler) where TCommand : ICommand
        {
            Handlers.AddOrUpdate(typeof(TCommand), handler, (key, oldValue) => handler);
        }

        public ICommandHandler Resolve<TCommand>() where TCommand : ICommand
        {
            var type = typeof(TCommand);
            if (Handlers.TryGetValue(type, out ICommandHandler handler))
                return handler;

            handler = ResolveGeneric(typeof(ICommandHandler<>), type);
            this.Register<TCommand>(handler);
            return handler;
        }

        public override ICommandHandler Resolve(ICommand cmd)
        {
            var type = cmd.GetType();
            if (Handlers.TryGetValue(type, out ICommandHandler handler))
                return handler;

            handler = this.ResolveGeneric(typeof(ICommandHandler<>), type);
            this.Register(type, handler);
            return handler;
        }
    }
}