using System;

namespace Assets 
{ 
    internal enum EdgeworkComparingMethod
    {
        Greater,
        Lower,
        Alphabetical,
        ReverseAlphabetical
    }

    internal static class EdgeworkComparingMethodExtensions
    {
        private static readonly int[] numbersAlphabeticalOrder = { 19, 9, 18, 16, 5, 4, 12, 10, 0, 7, 14, 2, 17, 15, 6, 3, 13, 11, 1, 8 };

        public static bool Compare(this EdgeworkComparingMethod comparer, int value1, int value2)
        {
            switch (comparer)
            {
                case EdgeworkComparingMethod.Greater:
                    return value1 >= value2;
                case EdgeworkComparingMethod.Lower:
                    return value1 <= value2;
                case EdgeworkComparingMethod.Alphabetical:
                    return numbersAlphabeticalOrder[value1 % 20] <= numbersAlphabeticalOrder[value2 % 20];
                case EdgeworkComparingMethod.ReverseAlphabetical:
                    return numbersAlphabeticalOrder[value1 % 20] >= numbersAlphabeticalOrder[value2 % 20];
                default:
                    throw new ArgumentOutOfRangeException("comparer", comparer, null);
            }
        }
    }
}