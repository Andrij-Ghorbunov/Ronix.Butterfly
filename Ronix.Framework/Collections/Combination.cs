using System;
using System.Collections.Generic;
using System.Linq;

namespace Ronix.Framework.Collections
{
    /// <summary>
    /// Contains static methods to operate with combinations.
    /// </summary>
    public static class Combination
    {
        /// <summary>
        /// Enumerates all combinations of P items in set of N items.
        /// <para> Each combination is represented with a bool array, where each element shows whether the item in its position is included into subset.</para>
        /// <para> The output is produced in lexicographical order.</para>
        /// </summary>
        /// <param name="total">N, the total number of elements in a set. Each of the produced arrays will have length of N.
        /// Should be nonnegative number.</param>
        /// <param name="group">P, the number of elements in a subset. Each of the produced arrays will contain exactly P items with true value.
        /// Should be nonnegative number less or equal to N.</param>
        /// <returns>Enumeration of all combinations of P from N.</returns>
        public static IEnumerable<bool[]> Enumerate(int total, int group)
        {
            if (total < 0)
                throw new ArgumentOutOfRangeException(nameof(total), "Total should be nonnegative");
            if (group < 0)
                throw new ArgumentOutOfRangeException(nameof(group), "Group size should be nonnegative");
            if (total < group)
                throw new ArgumentException("Group size should be less or equal to total");
            return EnumerateValidated(total, group).Select(it => it.ToArray());
        }

        private static IEnumerable<IEnumerable<bool>> EnumerateValidated(int total, int group)
        {
            if (total == group)
            {
                yield return Enumerable.Repeat(true, total);
                yield break;
            }
            if (group == 0)
            {
                yield return Enumerable.Repeat(false, total);
                yield break;
            }
            var limit = total - group + 1;
            for (var i = 0; i < limit; i++)
            {
                var prefix = Enumerable.Repeat(false, i).ToList();
                prefix.Add(true);
                foreach (var inner in EnumerateValidated(total - 1 - i, group - 1))
                {
                    yield return prefix.Concat(inner);
                }
            }
        }
    }
}
