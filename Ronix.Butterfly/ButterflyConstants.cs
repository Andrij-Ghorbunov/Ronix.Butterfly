using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly
{
    public static class ButterflyConstants
    {
        /// <summary>
        /// Permutation to flip the board between white and black sides.
        /// </summary>
        public static readonly int[] Flip = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 20, 24, 28, 32, 21, 25, 29, 33, 22, 26, 30, 34, 23, 27, 31, 35 };
        public const byte White = 1;
        public const byte Black = 2;
        public static byte FlipCell(byte cell)
        {
            if (cell == White) return Black;
            if (cell == Black) return White;
            return cell;
        }
    }
}
