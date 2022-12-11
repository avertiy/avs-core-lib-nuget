using System;

namespace AVS.CoreLib.PowerConsole.Enums
{
    /// <summary>
    /// Basic enumeration of message statuses, analog of LogLevel same purpose
    /// to distinct different levels of messages 
    /// </summary>
    [Flags]
    public enum MessageStatus
    {
        Default = 0,
        Debug = 1,
        Info = 2,
        Warning = 4,
        Error = 8
    }
}