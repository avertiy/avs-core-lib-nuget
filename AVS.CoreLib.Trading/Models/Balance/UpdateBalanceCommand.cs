namespace AVS.CoreLib.Trading.Models.Balance
{
    public class UpdateBalanceCommand
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public UpdateBalanceCommandType Type { get; set; }

    }
}