namespace Ronix.Butterfly
{
    /// <summary>
    /// Represents a compactified and performance-maximized version of the Butterfly game.
    /// </summary>
    public class ButterflyGame
    {
        /// <summary>
        /// Each 4 consecutive numbers represent the same (symmetric) move by white and black pieces.
        /// First four: 0, 2, 10, 12 - means that move 0 is white cell 0 to cell 2 or black cell 10 to cell 12, and so on.
        /// So, move # moveIndex is from cell Moves[4 * moveIndex + white?0:2] to cell Moves[4 * moveIndex + white?0:2 + 1].
        /// 255 represents no target cell (removal of the piece from the board).
        /// </summary>
        public static readonly int[] Moves = new[] { 0, 2, 10, 12, 1, 4, 11, 14, 2, 5, 12, 15, 3, 7, 13, 17, 4, 8, 14, 18, 5, 9, 15, 19, 9, 20, 19, 20, 8, 24, 18, 21, 7, 28, 17, 22, 6, 32, 16, 23, 20, 21, 20, 24, 24, 25, 21, 25, 28, 29, 22, 26, 32, 33, 23, 27, 21, 22, 24, 28, 25, 26, 25, 29, 29, 30, 26, 30, 33, 34, 27, 31, 22, 23, 28, 32, 26, 27, 29, 33, 30, 31, 30, 34, 34, 35, 31, 35, 35, 255, 35, 255, 31, 255, 34, 255, 27, 255, 33, 255, 23, 255, 32, 255, 9, 24, 19, 21, 8, 28, 18, 22, 7, 32, 17, 23, 6, 28, 16, 22, 7, 24, 17, 21, 8, 20, 18, 20, 20, 25, 20, 25, 24, 29, 21, 26, 28, 33, 22, 27, 32, 29, 23, 26, 28, 25, 22, 25, 24, 21, 21, 24, 21, 26, 24, 29, 25, 30, 25, 30, 29, 34, 26, 31, 33, 30, 27, 30, 29, 26, 26, 29, 25, 22, 25, 28, 22, 27, 28, 33, 26, 31, 29, 34, 30, 35, 30, 35, 34, 31, 31, 34, 30, 27, 30, 33, 26, 23, 29, 32 };

        public readonly byte[] Field;

        /// <summary>
        /// True = white, false = black
        /// </summary>
        public bool MoveSide { get; private set; }
        public int ScoreWhite { get; private set; }
        public int ScoreBlack { get; private set; }

        public bool[] WhiteMovesPossible { get; private set; }
        public bool[] BlackMovesPossible { get; private set; }

        public ButterflyGame()
        {
            Field = new byte[36];
            WhiteMovesPossible = new bool[50];
            BlackMovesPossible = new bool[50];
            Reset();
        }

        public ButterflyGame(ButterflyGame other)
        {
            Field = new byte[36];
            Array.Copy(other.Field, Field, 36);
            MoveSide = other.MoveSide;
        }

        public void Reset()
        {
            for (var i = 0; i < 10; i++)
            {
                Field[i] = ButterflyConstants.White;
            }
            for (var i = 10; i < 20; i++)
            {
                Field[i] = ButterflyConstants.Black;
            }
            for (var i = 20; i < 36; i++)
            {
                Field[i] = 0;
            }
            ScoreWhite = 0;
            ScoreBlack = 0;
            MoveSide = true;
        }

        public bool IsMovePossible(int index)
        {
            var startIndex = 4 * index + (MoveSide ? 0 : 2);
            var fromFieldIndex = Moves[startIndex];
            var fromFieldValue = Field[fromFieldIndex];
            // if the field doesn't contain our piece, we cannot move
            if (fromFieldValue != (MoveSide ? ButterflyConstants.White : ButterflyConstants.Black)) return false;
            var toFieldIndex = Moves[startIndex + 1];
            var toFieldValue = toFieldIndex == 255 ? 0 : Field[toFieldIndex];
            if (index < 22) // development or pushing
                return toFieldValue == 0; // target cell must be empty
            if (index < 26) // score
                return true; // always possible if the source field contains our piece
            return toFieldValue == (MoveSide ? ButterflyConstants.Black : ButterflyConstants.White); // attack or ambush - target field must contain enemy piece
        }

        public void Move(int index)
        {
            var startIndex = 4 * index + (MoveSide ? 0 : 2);
            var fromFieldIndex = Moves[startIndex];
            Field[fromFieldIndex] = 0; // source field emptied
            if (index > 21 && index < 26) // score
            {
                if (MoveSide)
                    ScoreWhite++;
                else
                    ScoreBlack++;
                MoveSide = !MoveSide;
                return;
            }
            Field[Moves[startIndex + 1]] = MoveSide ? ButterflyConstants.White : ButterflyConstants.Black;
            MoveSide = !MoveSide;
        }

        public bool CheckIfGameEnded()
        {
            var arrayToRefresh = MoveSide ? WhiteMovesPossible : BlackMovesPossible;
            var isAnyMovePossible = false;
            for (var i = 0; i < 50; i++)
            {
                var isMovePossible = IsMovePossible(i);
                arrayToRefresh[i] = isMovePossible;
                if (isMovePossible) isAnyMovePossible = true;
            }
            return !isAnyMovePossible;
        }

        /// <summary>
        /// Positive value = white won, negative value = black won, 0 = draw.
        /// <para>
        /// Detailed: hundreds show difference in Score Points, and 1s show difference in Development.
        /// E. g. +203 means white has won by 2 Score Points but also had 3 points advantage in Development;
        /// -197 means Black has won by 2 Score Points, at the same time lagging 3 points in Development behind the opponent.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int CheckWinner()
        {
            return 100 * (ScoreWhite - ScoreBlack) + GetDevelopment();
        }

        public int GetDevelopment()
        {
            var countDevelopedPieces = 0;
            for (var index = 20; index < 36; index++)
            {
                var piece = Field[index];
                if (piece == ButterflyConstants.White) countDevelopedPieces++;
                if (piece == ButterflyConstants.Black) countDevelopedPieces--;
            }
            return countDevelopedPieces;
        }
    }
}