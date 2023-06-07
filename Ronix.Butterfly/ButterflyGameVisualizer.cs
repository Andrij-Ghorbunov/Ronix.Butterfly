using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly
{
    /// <summary>
    /// Represents a Butterfly game in a more visual view, with reference to the physical board.
    /// </summary>
    public class ButterflyGameVisualizer
    {
        /// <summary>
        /// Indices of optimized array shaped as visual array.
        /// </summary>
        public static readonly int[,] VisualToOptimized = new[,] {
            {-1,-1,-1, 6,32,33,34,35},
            {-1,-1, 3, 7,28,29,30,31},
            {-1, 1, 4, 8,24,25,26,27},
            { 0, 2, 5, 9,20,21,22,23},
            {-1,-1,-1,-1,19,18,17,16},
            {-1,-1,-1,-1,15,14,13,-1},
            {-1,-1,-1,-1,12,11,-1,-1},
            {-1,-1,-1,-1,10,-1,-1,-1}
        };

        /// <summary>
        /// Indices of visual array shaped as optimized array.
        /// </summary>
        public static readonly int[,] OptimizedToVisual = InvertVisualToOptimized(VisualToOptimized);

        private static int[,] InvertVisualToOptimized(int[,] data)
        {
            var r = new int[50, 2];
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var optIndex = data[i, j];
                    if (optIndex != -1)
                    {
                        r[optIndex, 0] = i;
                        r[optIndex, 1] = j;
                    }
                }
            }
            return r;
        }

        private int GetDevelopment(byte side)
        {
            var r = 0;
            for (var i = 0; i < 4; i++)
            {
                for (var j = 4; j < 8; j++)
                {
                    if (Game.Field[VisualToOptimized[i, j]] == side)
                        r++;
                }
            }
            return r;
        }

        public int GetDevelopmentWhite()
        {
            return GetDevelopment(1);
        }

        public int GetDevelopmentBlack()
        {
            return GetDevelopment(2);
        }

        public ButterflyGame Game { get; }

        public List<GamePiece> Pieces { get; }

        public ButterflyGameVisualizer(ButterflyGame game)
        {
            Game = game;
            Pieces = new List<GamePiece>();
            for (var index = 0; index < 36; index++)
            {
                var piece = game.Field[index];
                if (piece != 0)
                {
                    Pieces.Add(new GamePiece { Side = piece == ButterflyConstants.White, X = OptimizedToVisual[index, 0], Y = OptimizedToVisual[index, 1] });
                }
            }
        }

        public byte GetPieceSide(int x, int y)
        {
            if (x < 0 || x > 7 || y < 0 || y > 7) return 0;
            var index = VisualToOptimized[x, y];
            if (index == -1) return 0;
            return Game.Field[index];
        }

        public IEnumerable<PossibleMove> GetPossibleMoves(GamePiece piece)
        {
            if (piece.Side) // white - left to right
            {
                if (piece.Y == 7)
                {
                    yield return PossibleMove.Score;
                    yield break;
                }
                if (GetPieceSide(piece.X, piece.Y + 1) == 0) // forward
                {
                    yield return new PossibleMove(piece.X, piece.Y + 1, false);
                }
                if (GetPieceSide(piece.X - 1, piece.Y + 1) == 2) // attack
                {
                    yield return new PossibleMove(piece.X - 1, piece.Y + 1, true);
                }
                if (piece.X < 3 && GetPieceSide(piece.X + 1, piece.Y + 1) == 2) // attack
                {
                    yield return new PossibleMove(piece.X + 1, piece.Y + 1, true);
                }
            }
            else // black - bottom to top
            {
                if (piece.X == 0)
                {
                    yield return PossibleMove.Score;
                    yield break;
                }
                if (GetPieceSide(piece.X - 1, piece.Y) == 0) // forward
                {
                    yield return new PossibleMove(piece.X - 1, piece.Y, false);
                }
                if (piece.Y > 4 && GetPieceSide(piece.X - 1, piece.Y - 1) == 1) // attack
                {
                    yield return new PossibleMove(piece.X - 1, piece.Y - 1, true);
                }
                if (GetPieceSide(piece.X - 1, piece.Y + 1) == 1) // attack
                {
                    yield return new PossibleMove(piece.X - 1, piece.Y + 1, true);
                }
            }
        }

        public void Move(GamePiece source, PossibleMove move)
        {
            var fromIndex = VisualToOptimized[source.X, source.Y];
            var toIndex = move.IsScore ? 255 : VisualToOptimized[move.X, move.Y];
            Move(fromIndex, toIndex);
            if (move.IsScore)
            {
                source.Remove();
            }
            else
            {
                source.Move(move.X, move.Y);
            }
        }

        public void MoveAttack(GamePiece source, GamePiece target)
        {
            var fromIndex = VisualToOptimized[source.X, source.Y];
            var toIndex = VisualToOptimized[target.X, target.Y];
            Move(fromIndex, toIndex);
            source.Move(target.X, target.Y);
            target.Remove();
        }

        private void Move(int fromIndex, int toIndex)
        {
            var moveIndex = GetMoveIndex(Game.MoveSide, fromIndex, toIndex);
            if (!Game.IsMovePossible(moveIndex))
                throw new Exception("Illegal move");
            Move(moveIndex);
        }

        private int GetMoveIndex(bool side, int fromIndex, int toIndex)
        {
            var sideInt = side ? 0 : 2;
            for (var index = 0; index < 50; index++)
            {
                if (ButterflyGame.Moves[4 * index + sideInt] == fromIndex && ButterflyGame.Moves[4 * index + sideInt + 1] == toIndex)
                    return index;
            }
            sideInt = side ? 2 : 0;
            for (var index = 0; index < 50; index++)
            {
                if (ButterflyGame.Moves[4 * index + sideInt] == fromIndex && ButterflyGame.Moves[4 * index + sideInt + 1] == toIndex)
                    throw new Exception("Move from wrong side attempted");
            }
            throw new Exception("Move not found");
        }

        public FullMove GetMoveFromIndex(int index)
        {
            var sideInt = Game.MoveSide ? 0 : 2;
            var from = ButterflyGame.Moves[4 * index + sideInt];
            var to = ButterflyGame.Moves[4 * index + sideInt + 1];
            var fromX = OptimizedToVisual[from, 0];
            var fromY = OptimizedToVisual[from, 1];
            var toX = to == 255 ? -1 : OptimizedToVisual[to, 0];
            var toY = to == 255 ? -1 : OptimizedToVisual[to, 1];
            var isScore = index > 21 && index < 26;
            return new FullMove
            {
                FromX = fromX,
                FromY = fromY,
                ToX = toX,
                ToY = toY,
                IsScore = isScore
            };
        }

        public void Move(int index)
        {
            var state = SaveGameState();
            var scoreSum = Game.ScoreWhite + Game.ScoreBlack;
            var scoreDiff = Game.ScoreWhite - Game.ScoreBlack;
            if (!Game.MoveSide) scoreDiff *= -1;
            var item = new DatasetItem(state, scoreSum, scoreDiff, index);
            if (Game.MoveSide)
                WhiteMoves.Add(item);
            else
                BlackMoves.Add(item);

            Game.Move(index);
        }

        public readonly List<DatasetItem> WhiteMoves = new List<DatasetItem>();
        public readonly List<DatasetItem> BlackMoves = new List<DatasetItem>();

        private byte[] SaveGameState()
        {
            var r = new byte[36];
            for (var i = 0; i < 36; i++)
            {
                var index = Game.MoveSide ? i : ButterflyConstants.Flip[i];
                r[index] = Game.MoveSide ? Game.Field[i] : ButterflyConstants.FlipCell(Game.Field[i]);
            }
            return r;
        }
    }
}
