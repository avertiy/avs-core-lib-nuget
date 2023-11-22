namespace AVS.CoreLib.Guards
{
    public class Guard : IMustBeGuardClause, IAgainstGuardClause, IMustGuardClause, IArrayGuardClause, IDictionaryGuardClause
    {
        private Guard() { }

        private static readonly Guard _guard = new Guard();
        /// <summary>
        /// An entry point to a set of Guard Clauses.
        /// </summary>
        public static IAgainstGuardClause Against { get; set; } = _guard;

        //public static IMustGuardClause Must { get; set; } = _guard;
        public static IMustBeGuardClause MustBe { get; set; } = _guard;
        public static IArrayGuardClause Array { get; set; } = _guard;
        /// <summary>
        /// for now this is just for a convenience but later might separate out guard list extensions
        /// </summary>
        public static IArrayGuardClause List { get; set; } = _guard;
        public static IDictionaryGuardClause Dictionary { get; set; } = _guard;
    }
}
