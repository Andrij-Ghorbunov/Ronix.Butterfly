namespace Ronix.Framework.Converters
{
    /// <summary>
    /// Contains methods to transform integers into ordinal string representation.
    /// </summary>
    public static class OrdinalEndings
    {
        /// <summary>
        /// Gets integer ordinal ending (st, nd, rd or th).
        /// </summary>
        /// <param name="number">Integer.</param>
        /// <returns>Ordinal ending.</returns>
        public static string Ending(int number)
        {
            if (number < 0)
                return Ending(-number);
            switch (number % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
                default:
                    switch (number % 10)
                    {
                        case 1: return "st";
                        case 2: return "nd";
                        case 3: return "rd";
                        default: return "th";
                    }
            }
        }

        /// <summary>
        /// Converts integer to its ordinal string representation (1 -> 1st, 11 -> 11th, 23 -> 23rd etc).
        /// </summary>
        /// <param name="number">Integer.</param>
        /// <returns>Ordinal representation.</returns>
        public static string ToOrdinal(int number)
        {
            return number + Ending(number);
        }
    }
}
