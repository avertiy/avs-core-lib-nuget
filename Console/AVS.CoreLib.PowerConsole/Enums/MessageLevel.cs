using System;

namespace AVS.CoreLib.PowerConsole.Enums
{
    /// <summary>
    /// Basic enumeration of message statuses, analog of LogLevel same purpose
    /// to distinct different levels of messages 
    /// </summary>
    public enum MessageLevel
    {
        Default = 0,
        Debug = 1,
        Info = 2,
        Success = 3,
        Important = 4,
        Warning = 5,
        Error = 6,
        //StackTrace = 7,
        Critical = 7
    }
}