namespace AVS.CoreLib.Trading.Enums
{
    public enum BarSize
    {
        Normal = 0,
        /// <summary>
        /// short bar is a bar with a little price difference between Low and High
        /// </summary>
        Short = 1,
        /// <summary>
        /// Long bar is a bar with a significant a few (2-3) times bigger than average bar
        /// </summary>
        Long = 2,
        /// <summary>
        /// Extreme big or paranormal bar is a bar with a significant length several times bigger than average bar
        /// </summary>
        Paranormal = 2,
    }

    public enum BarType
    {
        Bearish = 0,
        Bullish = 1,
    }
}