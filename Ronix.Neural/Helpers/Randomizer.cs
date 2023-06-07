using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Neural.Helpers
{
    public static class Randomizer
    {
        public static int[] Shuffle(int count, Random random)
        {
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
            return arr;
        }
    }
}
