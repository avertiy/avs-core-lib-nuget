namespace AVS.CoreLib.Enums
{
    public enum TernaryResult
    {
        No = -1,
        Undefined = 0,
        Yes = 1,
    }

    public static class TernaryResultExtensions
    {
        public static bool IsTrue(this TernaryResult res) => res == TernaryResult.Yes;
        public static bool IsFalse(this TernaryResult res) => res == TernaryResult.No;
    }
}
