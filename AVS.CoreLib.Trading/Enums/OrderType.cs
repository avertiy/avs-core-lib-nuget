namespace AVS.CoreLib.Trading.Enums
{
    public enum OrderType
    {
        Limit = 0,
        Market = 1,
        Immediate = 2,
        FillOrKill,
        GoodTillCanceled,
        StopLimit,
        Stop,
        OneCancelsTheOther
    }
}