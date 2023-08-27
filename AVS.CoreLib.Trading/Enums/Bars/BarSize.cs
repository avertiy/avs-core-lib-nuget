namespace AVS.CoreLib.Trading.Enums
{
    /// <summary>
    /// Classify bar by length 
    /// </summary>
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

    public enum VolumeSize
    {
        Normal = 0,        
        /// <summary>
        /// 2 times smaller volume than the average volume for the last 12 bars
        /// </summary>
        Reduced = 1,
        /// <summary>
        /// 2 times bigger volume than the average volume for the last 12 bars
        /// </summary>
        Increased = 2,
        Extreme = 3,
    }
}