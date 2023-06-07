using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly
{
    public class PossibleMove
    {
        public int X { get; }
        public int Y { get; }
        public bool IsScore { get; }
        public bool IsAttack { get; }

        public PossibleMove(int x, int y, bool isAttack)
        {
            X = x;
            Y = y;
            IsAttack = isAttack;
        }

        private PossibleMove()
        {
            IsScore = true;
        }

        public static readonly PossibleMove Score = new PossibleMove();
    }
}
