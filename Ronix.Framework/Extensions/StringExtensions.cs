using System;

namespace Ronix.Framework.Extensions
{
    /// <summary>
    /// Extensions for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets a part of the current string beginning from the first occurence of the given sequence (or <see cref="string.Empty"/> if it doesn't contain the sequence).
        /// </summary>
        /// <param name="this">The current string.</param>
        /// <param name="sequence">The sequence to search.</param>
        /// <returns>Current string starting with the sequence.</returns>
        public static string From(this string @this, string sequence)
        {
            var index = @this.IndexOf(sequence, StringComparison.Ordinal);
            return index == -1 ? string.Empty : @this.Substring(index);
        }

        /// <summary>
        /// Gets a part of the current string ending at the last occurence of the given sequence (or <see cref="string.Empty"/> if it doesn't contain the sequence).
        /// </summary>
        /// <param name="this">The current string.</param>
        /// <param name="sequence">The sequence to search.</param>
        /// <returns>Current string ending with the sequence.</returns>
        public static string To(this string @this, string sequence)
        {
            var index = @this.LastIndexOf(sequence, StringComparison.Ordinal);
            return index == -1 ? string.Empty : @this.Substring(0, index + sequence.Length);
        }

        /// <summary>
        /// Gets a part of the current string after the first occurence of the given sequence (or <see cref="string.Empty"/> if it doesn't contain the sequence).
        /// </summary>
        /// <param name="this">The current string.</param>
        /// <param name="sequence">The sequence to search.</param>
        /// <returns>Current string after the sequence.</returns>
        public static string After(this string @this, string sequence)
        {
            var index = @this.IndexOf(sequence, StringComparison.Ordinal);
            return index == -1 ? string.Empty : @this.Substring(index + sequence.Length);
        }

        /// <summary>
        /// Gets a part of the current string before the last occurence of the given sequence (or <see cref="string.Empty"/> if it doesn't contain the sequence).
        /// </summary>
        /// <param name="this">The current string.</param>
        /// <param name="sequence">The sequence to search.</param>
        /// <returns>Current string before the sequence.</returns>
        public static string Before(this string @this, string sequence)
        {
            var index = @this.LastIndexOf(sequence, StringComparison.Ordinal);
            return index == -1 ? string.Empty : @this.Substring(0, index);
        }

        /// <summary>
        /// Gets a part of the current string beginning from the last occurence of the given sequence (or <see cref="string.Empty"/> if it doesn't contain the sequence).
        /// </summary>
        /// <param name="this">The current string.</param>
        /// <param name="sequence">The sequence to search.</param>
        /// <returns>Current string starting with the sequence.</returns>
        public static string FromLast(this string @this, string sequence)
        {
            var index = @this.LastIndexOf(sequence, StringComparison.Ordinal);
            return index == -1 ? string.Empty : @this.Substring(index);
        }

        /// <summary>
        /// Gets a part of the current string ending at the first occurence of the given sequence (or <see cref="string.Empty"/> if it doesn't contain the sequence).
        /// </summary>
        /// <param name="this">The current string.</param>
        /// <param name="sequence">The sequence to search.</param>
        /// <returns>Current string ending with the sequence.</returns>
        public static string ToFirst(this string @this, string sequence)
        {
            var index = @this.IndexOf(sequence, StringComparison.Ordinal);
            return index == -1 ? string.Empty : @this.Substring(0, index + sequence.Length);
        }

        /// <summary>
        /// Gets a part of the current string after the last occurence of the given sequence (or <see cref="string.Empty"/> if it doesn't contain the sequence).
        /// </summary>
        /// <param name="this">The current string.</param>
        /// <param name="sequence">The sequence to search.</param>
        /// <returns>Current string after the sequence.</returns>
        public static string AfterLast(this string @this, string sequence)
        {
            var index = @this.LastIndexOf(sequence, StringComparison.Ordinal);
            return index == -1 ? string.Empty : @this.Substring(index + sequence.Length);
        }

        /// <summary>
        /// Gets a part of the current string before the first occurence of the given sequence (or <see cref="string.Empty"/> if it doesn't contain the sequence).
        /// </summary>
        /// <param name="this">The current string.</param>
        /// <param name="sequence">The sequence to search.</param>
        /// <returns>Current string before the sequence.</returns>
        public static string BeforeFirst(this string @this, string sequence)
        {
            var index = @this.IndexOf(sequence, StringComparison.Ordinal);
            return index == -1 ? string.Empty : @this.Substring(0, index);
        }
    }
}
