using System;
using System.Linq;

namespace Ronix.Framework.Converters
{
    /// <summary>
    /// Enumeration of Roman number formats.
    /// </summary>
    public enum RomanNumberFormat
    {
        /// <summary>
        /// Common format can be used to write numbers up to 3999.
        /// </summary>
        Common,

        /// <summary>
        /// Same as <see cref="Common"/>, but when parsing from Roman, the case of the input is ignored.
        /// </summary>
        CommonIgnoreCase,
        
        /// <summary>
        /// Can be used to write any integer number. Uses notation (Billions)m(Millions)m(Thousands)m(Ones), e. g. 2117325 is IImCXVIImCCCXXV.
        /// </summary>
        ThousandsDelimited
    }
    
    /// <summary>
    /// Contains methods to translate integers to and from Roman number format.
    /// </summary>
    public static class RomanNumbers
    {
        #region Atoms

        private struct Atom
        {
            internal readonly string Roman;
            internal readonly int Value;

            internal Atom(string roman, int value)
            {
                Roman = roman;
                Value = value;
            }
        }

        private static readonly Atom[] Atoms = new[]
                                          {
                                              new Atom("M", 1000), new Atom("CM", 900), new Atom("D", 500),
                                              new Atom("CD", 400), new Atom("C", 100), new Atom("XC", 90),
                                              new Atom("L", 50), new Atom("XL", 40), new Atom("X", 10), new Atom("IX", 9),
                                              new Atom("V", 5), new Atom("IV", 4), new Atom("I", 1)
                                          };

        #endregion Atoms

        #region Public

        /// <summary>
        /// Converts integer into Roman representation using <see cref="RomanNumberFormat.Common"/> format.
        /// </summary>
        /// <param name="number">Integer.</param>
        /// <returns>Roman representation.</returns>
        public static string ToRoman(int number)
        {
            return ToRoman(number, RomanNumberFormat.Common);
        }

        /// <summary>
        /// Converts integer into Roman representation using the provided format.
        /// </summary>
        /// <param name="number">Integer.</param>
        /// <param name="format">The format.</param>
        /// <returns>Roman representation.</returns>
        public static string ToRoman(int number, RomanNumberFormat format)
        {
            if (number == 0)
                return string.Empty;
            if (number < 0)
                return string.Format("-{0}", ToRoman(-number));
            if (number > 3999 || (number > 999 && format == RomanNumberFormat.ThousandsDelimited))
            {
                if (format != RomanNumberFormat.ThousandsDelimited)
                    throw new ArgumentOutOfRangeException("number");
                return ToRomanThousands(number);
            }
            foreach (var atom in Atoms)
            {
                if (number >= atom.Value) return atom.Roman + ToRoman(number - atom.Value);
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts Roman number into integer using <see cref="RomanNumberFormat.Common"/> format.
        /// </summary>
        /// <param name="roman">Roman representation.</param>
        /// <returns>Integer.</returns>
        public static int FromRoman(string roman)
        {
            return FromRoman(roman, RomanNumberFormat.Common);
        }

        /// <summary>
        /// Converts Roman number into integer using the provided format.
        /// </summary>
        /// <param name="roman">Roman representation.</param>
        /// <param name="format">The format.</param>
        /// <returns>Integer.</returns>
        public static int FromRoman(string roman, RomanNumberFormat format)
        {
            if (string.IsNullOrEmpty(roman))
                return 0;
            if (roman.StartsWith("-"))
                return -FromRoman(roman.Substring(1));
            switch (format)
            {
                case RomanNumberFormat.Common: return FromRomanCommon(roman);
                case RomanNumberFormat.CommonIgnoreCase: return FromRomanCommon(roman.ToUpperInvariant());
                case RomanNumberFormat.ThousandsDelimited: return FromRomanThousandsDelimited(roman);
            }
            throw new ArgumentOutOfRangeException("format");
        }

        #endregion Public

        #region Private

        private static string ToRomanThousands(int number)
        {
            var units = number % 1000;
            var thousands = number / 1000;
            return string.Format("{0}m{1}", ToRoman(thousands, RomanNumberFormat.ThousandsDelimited), ToRoman(units, RomanNumberFormat.ThousandsDelimited));
        }

        private static int FromRomanCommon(string roman)
        {
            var r = 0;
            foreach (var atom in Atoms)
            {
                while (roman.StartsWith(atom.Roman))
                {
                    roman = roman.Substring(atom.Roman.Length);
                    r += atom.Value;
                }
            }
            return r;
        }

        private static int FromRomanThousandsDelimited(string roman)
        {
            var arr = roman.Split('m').Select(FromRomanCommon).ToArray();
            var t = 0;
            foreach (var x in arr)
            {
                t *= 1000;
                t += x;
            }
            return t;
        }

        #endregion Private
    }
}
