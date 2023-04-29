namespace AVS.CoreLib.Trading.Enums.Futures
{
    /// <summary>
    /// Position side
    /// </summary>
    public enum PositionSide
    {
        /// <summary>
        /// Both for one-way mode when placing an order
        /// </summary>
        Both = 0,
        /// <summary>
        /// Short used in two-way (hedge) mode
        /// </summary>
        Short,
        /// <summary>
        /// Long used in two-way (hedge) mode
        /// </summary>
        Long
    }
}