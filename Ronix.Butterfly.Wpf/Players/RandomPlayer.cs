using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Players
{
    public class RandomPlayer : PlayerBase
    {
        private static readonly Random Random = new Random();

        public override string Name => "Random";

        public override int? Prompt(ButterflyGame game)
        {
            return Move(game);
        }
        
        public int Move(ButterflyGame game)
        {
            var possibleMoves = game.MoveSide ? game.WhiteMovesPossible : game.BlackMovesPossible;
            var totalCount = possibleMoves.Count(it => it);
            var number = Random.Next(totalCount);
            var index = -1;
            while (number >= 0)
            {
                if (possibleMoves[++index]) number--;
            }
            return index;
        }

        public override PlayerBase Copy()
        {
            return this;
        }
    }
}
