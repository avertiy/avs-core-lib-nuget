namespace AVS.CoreLib.Enums
{
    /// <summary>
    /// universal enumeration to treat value in some way
    /// might be useful for json converters
    /// </summary>
    public enum ValueModifier
    {
        None = 0,
        /// <summary>
        /// Invert the original value
        /// </summary>
        Opposite = 1,
        /// <summary>
        /// absolute value for numbers for example -2 => 2
        /// </summary>
        Absolute,
        /// <summary>
        /// round value
        /// </summary>
        Round,
        /// <summary>
        /// round up
        /// </summary>
        Ceiling,
        /// <summary>
        /// round down
        /// </summary>
        Floor, 
        /// <summary>
        /// truncate value
        /// </summary>
        Truncate,
        /// <summary>
        /// treat value as decimal number
        /// </summary>
        Decimal,
        /// <summary>
        /// treat value as percentage
        /// </summary>
        Percentage
    }
}