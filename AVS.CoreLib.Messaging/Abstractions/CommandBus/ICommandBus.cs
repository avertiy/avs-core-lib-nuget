using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Messaging;

namespace AVS.CoreLib.Messaging.Abstractions.CommandBus
{
    /// <summary>
    /// Represents 1-to-1 model of message communication.
    /// Command bus passes <see cref="ICommand"/> to a <see cref="ICommandHandlerFactory"/>.
    /// The handler will perform the required actions. 
    /// A certain type of command is expected to be handled by exactly one command handler
    /// which resolved through <see cref="ICommandHandler"/>
    /// If there is no handler registered for the command, an exception will be thrown. 
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// Send expects <see cref="ICommand"/> to be consumed.
        /// If there is no consumer configured for the message type, an exception will be thrown.
        /// </summary>
        void Send<TCommand>(TCommand command) where TCommand : ICommand;
        void Send(params ICommand[] commands);
        Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand;
        Task SendAsync(params ICommand[] commands);
    }
}