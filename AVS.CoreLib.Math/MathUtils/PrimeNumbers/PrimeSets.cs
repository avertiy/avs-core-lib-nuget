using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Math.MathUtils.PrimeNumbers.Structs;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers
{
    public static class PrimeSets
    {
        private static List<Prime> _set1;

        public static List<Prime> Set65536
        {
            get
            {
                if (_set1 == null)
                {
                    _set1 = new List<Prime>();
                    _set1 = SmallPrimes.Set65536.Select(x => Prime.FromPrimeNumber((ulong)x)).ToList();
                }

                return _set1;
            }
        }

        /// <summary>
        /// Special primes #45 numbers out of int range from 1 to 2^24 [65536 * 256]
        /// </summary>
        //var specialSet = Primes.GeneratePrimeNumbers(1, 65536 * 256).Where(x => x.IsSpecialPrime()).ToList();
        //var arr = specialSet.AsArrayString(",");
        public static int[] SpecialPrimes = new int[]
        {
            294001, 505447, 584141,
            604171, 971767, 1062599,
            1282529, 1524181, 2017963,
            2474431, 2690201, 3085553,
            3326489, 4393139, 5152507,
            5564453, 5575259, 6173731,
            6191371, 6236179, 6463267,
            6712591, 7204777, 7469789,
            7469797, 7858771, 7982543,
            8090057, 8353427, 8532761,
            8639089, 9016079, 9537371,
            9608189, 9931447, 10506191,
            10564877, 11124403, 11593019,
            12325739, 14075273, 14090887,
            14151757, 15973733, 16497121
        };

        private static List<Prime> _set2;

        public static List<Prime> Set2
        {
            get
            {
                if (_set2 == null)
                {
                    _set2 = new List<Prime>();
                    _set2 = Primes.GeneratePrimeNumbers(65536, 65536 * 8).Select(x => Prime.FromPrimeNumber((ulong)x)).ToList();
                }

                return _set2;
            }
        }
    }
}