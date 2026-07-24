namespace AVS.CoreLib.Enums
{
    /// <summary>
    /// Instructs how to serialize object to array, object, plain string etc.
    /// </summary>
    public enum JsonFormat
    {
        None = 0,
        Value = 1,
        String = 2,
        Array = 3,
        Object =4,
        Plain = 5
    }
}