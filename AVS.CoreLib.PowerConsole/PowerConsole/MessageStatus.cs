using System;

namespace AVS.CoreLib.PowerConsole
{
    /// <summary>
    /// Basic enumeration of message statuses
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