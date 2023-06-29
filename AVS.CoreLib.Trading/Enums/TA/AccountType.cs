namespace AVS.CoreLib.Trading.Enums.TA
{
    public enum IndicatorType
    {
        /// <summary>
        /// same scale as asset price, plotted directly on a price chart. 
        /// </summary>
        Overlay,
        /// <summary>
        /// oscillate between a local minimum and maximum, plotted below price chart
        /// </summary>
        Oscillator
    }

    ///// <summary>
    ///// which price should be used to calculate indicator
    ///// </summary>
    //public enum Source
    //{
    //    Close = 0,
    //    Open =1,
    //    High =2,
    //    Low =3
    //}

    public enum MAType
    {
        SMA = 0,
        EMA = 1,
        WMA= 2,
        /// <summary>
        /// Hull MA
        /// </summary>
        HMA =3
    }

    public enum Indicator
    {
        SMA = 0,
        EMA = 1,
        WMA = 2,
        HMA = 3,
        BB,
        RSI
    }
}