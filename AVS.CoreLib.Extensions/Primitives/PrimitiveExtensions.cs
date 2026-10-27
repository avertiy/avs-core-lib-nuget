namespace AVS.CoreLib.Extensions
{
    public static class PrimitiveExtensions
    {
        public static bool IsTrue(this bool? value) => value.GetValueOrDefault();
        public static bool IsFalse(this bool? value) => value.GetValueOrDefault() == false;
    }
}