namespace AVS.CoreLib.Trading.Enums
{
    public enum OrderState
    {
        /// <summary>
        /// order has not been posted (not opened)
        /// </summary>
        Pending = 0,
        /// <summary>
        /// order has been posted (opened) on stock-exchange
        /// </summary>
        Open = 10,
        /// <summary>
        /// order is being processing by algorithm
        /// </summary>
        Processing = 20,
        /// <summary>
        /// order has been canceled (not executed)
        /// </summary>
        Canceled = 30,

        PartiallyExecuted = 40,
        /// <summary>
        /// order has been executed 
        /// </summary>
        Executed = 41,
        
        //Closed = 45,
        /// <summary>
        /// order is quite old marked as archived
        /// </summary>
        Archived = 50
    }
}