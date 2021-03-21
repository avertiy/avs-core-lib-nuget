namespace AVS.CoreLib.Abstractions.Messaging.CommandBus
{
    /// <summary>
    /// It is expected that a single service instance performs the command action.
    /// </summary>
    public interface ICommandHandler
    {
        void Handle(ICommand command);
    }

    /// <summary>
    /// It is expected that a single service instance performs the command action.
    /// </summary>
    public interface ICommandHandler<T> where T : ICommand
    {
    }

    /// <summary>
    /// It is expected 1 to 1 relation, i.e. for a certain command type the exact one <see cref="ICommandHandler"/> could be resolved.
    /// </summary>
    public interface ICommandHandlerFactory
    {
        /// <summary>
        /// Resolve <see cref="ICommandHandler"/> from DI container
        /// </summary>
        /// <returns>command handler or null if command's handler was not registered</returns>
        ICommandHandler Resolve<TCommand>() where TCommand : ICommand;

        /// <summary>
        /// Resolve <see cref="ICommandHandler"/> from DI container
        /// </summary>
        /// <returns>command handler or null if command's handler was not registered</returns>
        ICommandHandler Resolve(ICommand command);
    }
}