using System;

namespace AVS.CoreLib.Trading.Enums.TA
{
    /// <summary>
    /// indicator line slope
    /// moving average slope or stochastic etc. curve 
    /// </summary>
    public enum Slope
    {
        /// <summary>
        /// no clear slope detected
        /// </summary>
        None = 0,
        /// <summary>
        /// indicates that the curve clearly moves down fast
        /// </summary>
        SteepBearish = -20,
        /// <summary>
        /// indicates that the curve moves down slowly   
        /// </summary>
        Bearish = -10,
        /// <summary>
        /// indicates that the curve moves up slowly
        /// </summary>
        Bullish = 10,
        /// <summary>
        /// indicates that the curve clearly moves up fast
        /// </summary>
        SteepBullish = 20
    }

    [Flags]
    public enum HTFEnum
    {
        None = 0,
        HTF1 =1,
        HTF2 =2,
        HTF3 =4,
        ALL = HTF1|HTF2|HTF3
    }

    public enum TAProp
    {
        Close = 0,        
        High =1 ,
        Low =2,
        Open =3,
        Volume =4,
        HL2,
        HLC3,
        BodyLength,
        Length,
        AvgLength,
        AvgVolume,
    }

    public enum MarketPhase
    {
        /// <summary>
        /// Цены двигаются вверх, формируя Higher Highs and Higher Lows
        /// </summary>
        UpTrend,
        /// <summary>
        /// Цены двигаются вниз, формируя Lower Highs and Lower Lows
        /// </summary>
        DownTrend,
        /// <summary>
        /// Цены колеблются в узком диапазоне
        /// </summary>
        Range,
        /// <summary>
        /// рынок не показывает явного направления и двигается в пределах определенных уровней поддержки и сопротивления
        /// </summary>
        Swings,
        /// <summary>
        /// рынок временно замедляет движение после активного тренда
        /// </summary>
        Consolidation,
        /// <summary>
        /// коррекционное движение против общего направления тренда
        /// </summary>
        Retracement
    }

    public enum TrendStrength
    {
        None = 0,
        Weak,
        Strong,
        VeryStrong
    }

    public enum PriceZone
    {
        None,
        Buy,
        Sell,
        MidPoint,
        Overbought,
        Oversold,
    }

    public enum Signal
    {
        None = 0,
        Long = 1,
        Short = 2,
        Divergence =3,
        Crossover =4,
        /// <summary>
        /// Danger for cases when there are signs of possible agressive bearish/bullish movement
        /// к примеру волатильность сужена, а индикаторы типа ADX сигналят сильный даун тренд => следует сливная елда на -20%
        /// </summary>
        Danger =5,
        
    }
}