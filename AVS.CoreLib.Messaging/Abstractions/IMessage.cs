namespace AVS.CoreLib.Abstractions.Messaging
{
    /// <summary>
    /// There are two main message types: events and commands.
    /// <see cref="ICommand"/>
    /// <see cref="IEvent"/>
    ///  Messages should be limited to read-only properties and not include methods or behavior.
    /// </summary>
    public interface IMessage
    {
    }


    /// <summary>
    /// Command tells a service to do something. Commands are sent (using Send) by command bus.
    /// It is expected that a single service instance performs the command action.
    /// A command should never be published.
    /// Example Commands:
    ///  -UpdateCustomerAddress
    ///  -UpgradeCustomerAccount
    ///  -SubmitOrder
    /// </summary>
    public interface ICommand : IMessage { }

    /// <summary>
    /// Event signifies that something has happened.Events are published(using Publish).
    /// Example Events:
    ///  -CustomerAddressUpdated
    ///  -CustomerAccountUpgraded
    /// </summary>
    public interface IEvent : IMessage
    {
    }
}
