namespace AVS.CoreLib.Trading.Enums
{
    public enum BarSize
    {
        Normal = 0,
        /// <summary>
        /// short bar is a bar with a little price difference between Low and High
        /// </summary>
        Small = 1,
        /// <summary>
        /// Big or paranormal bar is a bar with a significant length several times bigger than average bar
        /// </summary>
        Paranormal = 2,
    }
}