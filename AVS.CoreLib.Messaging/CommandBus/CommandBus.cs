using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Messaging.Abstractions.CommandBus;

namespace AVS.CoreLib.Messaging.CommandBus
{
    /// <summary>
    /// Represents a message bus implementation of 1-to-1 model of message distribution.
    /// Published message is received by all consuming subscribers (message handlers). 
    /// Allow send / handle messages between services/components
    /// without requiring the components to explicitly be aware of each other. 
    /// Direct message(s) to corresponding message handler(s) by message type.
    /// Message handler(s) must be registered in DI and implement <see cref="ICommandHandler"/> or <see cref="T:IMessageHandler"/>
    /// </summary>
    class CommandBus : ICommandBus
    {
        private readonly ICommandHandlerFactory _factory;

        public CommandBus(ICommandHandlerFactory factory)
        {
            _factory = factory;
        }

        public Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            var task = Task.Run(() =>
            {
                var handler = _factory.Resolve<TCommand>();
                if (handler == null)
                    throw new SendCommandException($"CommandHandler for the {command.GetType().Name} not found");
                handler.Handle(command);
            });
            return task;
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = _factory.Resolve<TCommand>();
            if (handler == null)
                throw new SendCommandException($"CommandHandler for the {command.GetType().Name} not found");
            handler.Handle(command);
        }

        public void Send(params ICommand[] commands)
        {
            foreach (var command in commands)
            {
                var handler = _factory.Resolve(command);
                if (handler == null)
                    throw new SendCommandException($"CommandHandler for the {command.GetType().Name} not found");
                handler.Handle(command);
            }
        }

        public Task SendAsync(params ICommand[] commands)
        {
            var task = Task.Run(() =>
            {
                foreach (var command in commands)
                {
                    var handler = _factory.Resolve(command);
                    if (handler == null)
                        throw new SendCommandException($"CommandHandler for the {command.GetType().Name} not found");
                    handler.Handle(command);
                }
            });
            return task;
        }
    }
}