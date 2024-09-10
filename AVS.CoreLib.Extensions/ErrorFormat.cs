namespace AVS.CoreLib.Extensions;

public enum ErrorFormat
{
    /// <summary>
    /// don't cut anything
    /// </summary>
    None = 0,
    /// <summary>
    /// truncate text 
    /// </summary>
    Truncated = 1,
    /// <summary>
    /// a bit less truncated than <see cref="Truncated"/> option
    /// </summary>
    Shortcut = 2,
    /// <summary>
    /// format error details for console output
    /// </summary>
    Console = 3,
}