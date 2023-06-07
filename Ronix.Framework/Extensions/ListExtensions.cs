using System;
using System.Collections.Generic;
using System.Linq;

namespace Ronix.Framework.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IList{T}"/>.
    /// </summary>
    public static class ListExtensions
    {
        ///// <summary>
        ///// Recursive shuffle using IList.Insert.
        ///// </summary>
        ///// <typeparam name="T">Type of the list.</typeparam>
        ///// <param name="this">List to shuffle.</param>
        ///// <param name="random">Provider of random numbers to produce the shuffling order.</param>
        ///// <returns>The shuffled list.</returns>
        //public static IList<T> SlowShuffle<T>(this IList<T> @this, Random random)
        //{
        //    if (@this.Count < 1)
        //        return @this.ToList();
        //    var r = @this.Skip(1).ToList().SlowShuffle(random);
        //    r.Insert(random.Next(@this.Count), @this.First());
        //    return r;
        //}

        /// <summary>
        /// Straightforward shuffle forms a random array of indices.
        /// </summary>
        /// <typeparam name="T">Type of the list.</typeparam>
        /// <param name="this">List to shuffle.</param>
        /// <param name="random">Provider of random numbers to produce the shuffling order.</param>
        /// <returns>The shuffled list.</returns>
        public static IList<T> Shuffle<T>(this IList<T> @this, Random random)
        {
            var count = @this.Count;
            var c = count;
            var arr = new int[count];
            var taken = new bool[count];
            for (int i = 0; i < count; i++)
            {
                var rand = random.Next(c--) + 1;
                var index = -1;
                while (rand > 0)
                {
                    if (!taken[++index])
                        rand--;
                }
                arr[i] = index;
                taken[index] = true;
            }
            var r = new T[count];
            for (int i = 0; i < count; i++)
                r[i] = @this[arr[i]];
            return r.ToList();
        }

        /// <summary>
        /// Gets a random item from the list.
        /// </summary>
        /// <typeparam name="T">Type of the list.</typeparam>
        /// <param name="this">Source list.</param>
        /// <param name="random">Provider of random numbers to produce the item index.</param>
        /// <returns>A random item from the list.</returns>
        public static T Random<T>(this IList<T> @this, Random random)
        {
            return @this[random.Next(@this.Count)];
        }
    
        /// <summary>
        /// Gets a random item and removes it from the list.
        /// </summary>
        /// <typeparam name="T">Type of the list.</typeparam>
        /// <param name="this">Source list.</param>
        /// <param name="random">Provider of random numbers to produce the item index.</param>
        /// <returns>A random item from the list.</returns>
        public static T PickRandom<T>(this IList<T> @this, Random random)
        {
            var index = random.Next(@this.Count);
            var item = @this[index];
            @this.RemoveAt(index);
            return item;
        }
    }
}
