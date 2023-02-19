namespace AVS.CoreLib.Trading.Enums
{
    /// <summary>Represents a time frame of a market.</summary>
    public enum TimeFrame
    {
        None = 0,
        /// <summary> interval of 1 second </summary>
        S1 = 1,
        /// <summary> interval of 30 seconds </summary>
        S30 = 30,
        /// <summary>A time interval of 1 minute.</summary>
        M1 = 60,

        /// <summary>A time interval of 5 minutes.</summary>
        M5 = 300,

        /// <summary>A time interval of 15 minutes.</summary>
        M15 = 900,

        /// <summary>A time interval of 30 minutes.</summary>
        M30 = 1800,

        H1 = 3600,
        /// <summary>A time interval of 2 hours.</summary>
        H2 = 7200,

        H3 = 10800,

        /// <summary>A time interval of 4 hours.</summary>
        H4 = 14400,

        H12 = 43200,

        /// <summary>A time interval of a day.</summary>
        D = 86400,

        Week = 86400 * 7,

        Month = 86400 * 30
    }
}
