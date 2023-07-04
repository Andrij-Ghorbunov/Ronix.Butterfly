using Ronix.Butterfly.Wpf.Players;
using Ronix.Butterfly.Wpf.Views;
using Ronix.Neural;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Helpers
{
    public static class NeuralHelper
    {
        private static readonly double[] Input = new double[38];

        private const double OurPiece = 1;

        private const double EnemyPiece = -1;

        private const double Empty = 0;

        /// <summary>
        /// Correspond to indices 0, 1, 2 - as used by the engine
        /// </summary>
        private static readonly double[] Signals = new[] { Empty, OurPiece, EnemyPiece };
        
        /// <summary>
        /// As previous, but white and black pieces swapped
        /// </summary>
        private static readonly double[] FlipSignals = new[] { Empty, EnemyPiece, OurPiece };

        /// <summary>
        /// We only need one instance in memory, it has <see cref="ButterflyGame.Reset"/> method.
        /// </summary>
        private static readonly ButterflyGame GameInstance = new();

        public static NoiseVm Noise;

        public static int Move(ButterflyGame game, LayeredNeuralNetwork network)
        {
            if (network.InputNumber != 38)
                throw new Exception($"Don't know how to make a move for this number of output neurons: {network.InputNumber} (must be 38)");
            switch (network.OutputNumber)
            {
                case 1: return Move1(game, network);
                case 50: return Move50(game, network);
            }
            throw new Exception($"Don't know how to make a move for this number of output neurons: {network.OutputNumber} (must be either 1 or 50)");
        }
        private static int Move50(ButterflyGame game, LayeredNeuralNetwork network)
        {
            var possibleMoves = game.MoveSide ? game.WhiteMovesPossible : game.BlackMovesPossible;
            if (game.MoveSide) // for white
            {
                for (var index = 0; index < 36; index++)
                {
                    Input[index] = Signals[game.Field[index]];
                }
                Input[36] = game.ScoreWhite - game.ScoreBlack;
            }
            else // for black
            {
                for (var index = 0; index < 36; index++)
                {
                    // flip our and enemy, and also flip positions on the board
                    Input[index] = FlipSignals[game.Field[ButterflyConstants.Flip[index]]];
                }
                Input[36] = game.ScoreBlack - game.ScoreWhite;
            }
            Input[37] = game.ScoreWhite + game.ScoreBlack;

            Noise.ProcessInput(Input);
            var output = network.ComputeOutput(Input);
            Noise.ProcessOutput(output);

            var maxSignal = double.NegativeInfinity;
            var maxIndex = -1;
            // calculate index of maximum output signal among the legal moves only
            for (var move = 0; move < 50; move++)
            {
                if (possibleMoves[move])
                {
                    var outputSignal = output[move];
                    if (outputSignal > maxSignal)
                    {
                        maxSignal = outputSignal;
                        maxIndex = move;
                    }
                }
            }
            return maxIndex;
        }

        private static int Move1(ButterflyGame game, LayeredNeuralNetwork network)
        {
            var possibleMoves = game.MoveSide ? game.WhiteMovesPossible : game.BlackMovesPossible;
            var possibleMoveIndices = Enumerable.Range(0, 50).Where(it => possibleMoves[it]).ToList();
            if (possibleMoveIndices.Count == 1) return possibleMoveIndices[0];
            var maxSignal = double.NegativeInfinity;
            var maxSignalIndex = -1;
            foreach (var possibleMove in possibleMoveIndices)
            {
                var copyGame = new ButterflyGame(game);
                copyGame.Move(possibleMove);
                if (game.MoveSide) // for white
                {
                    for (var index = 0; index < 36; index++)
                    {
                        Input[index] = Signals[copyGame.Field[index]];
                    }
                    Input[36] = copyGame.ScoreWhite - copyGame.ScoreBlack;
                }
                else // for black
                {
                    for (var index = 0; index < 36; index++)
                    {
                        // flip our and enemy, and also flip positions on the board
                        Input[index] = FlipSignals[copyGame.Field[ButterflyConstants.Flip[index]]];
                    }
                    Input[36] = copyGame.ScoreBlack - copyGame.ScoreWhite;
                }
                Input[37] = copyGame.ScoreWhite + copyGame.ScoreBlack;
                Noise.ProcessInput(Input);
                var outputs = network.ComputeOutput(Input);
                Noise.ProcessOutput(outputs);
                var output = outputs[0];
                if (output > maxSignal)
                {
                    maxSignal = output;
                    maxSignalIndex = possibleMove;
                }
            }
            return maxSignalIndex;
        }

        public static int ContestTwoNetworks(LayeredNeuralNetwork white, LayeredNeuralNetwork black, bool isParallel = true)
        {
            var gameInstance = isParallel ? new ButterflyGame() : GameInstance;
            if (!isParallel)
                gameInstance.Reset();
            while (!gameInstance.CheckIfGameEnded())
            {
                var move = Move(gameInstance, white);
                gameInstance.Move(move);
                if (gameInstance.CheckIfGameEnded()) break;
                move = Move(gameInstance, black);
                gameInstance.Move(move);
            }
            return gameInstance.CheckWinner();
        }

        public static int ContestNetworkAndRandom(LayeredNeuralNetwork network, RandomPlayer random, bool isParallel = false)
        {
            var gameInstance = isParallel ? new ButterflyGame() : GameInstance;
            if (!isParallel)
                gameInstance.Reset();
            while (!gameInstance.CheckIfGameEnded())
            {
                var move = Move(gameInstance, network);
                gameInstance.Move(move);
                if (gameInstance.CheckIfGameEnded()) break;
                move = random.Move(gameInstance);
                gameInstance.Move(move);
            }
            var winnerWhite = gameInstance.CheckWinner();
            gameInstance.Reset();
            while (!gameInstance.CheckIfGameEnded())
            {
                var move = random.Move(gameInstance);
                gameInstance.Move(move);
                if (gameInstance.CheckIfGameEnded()) break;
                move = Move(gameInstance, network);
                gameInstance.Move(move);
            }
            var winnerBlack = gameInstance.CheckWinner();
            return Math.Sign(winnerWhite) - Math.Sign(winnerBlack);
        }

        /// <summary>
        /// Simply check who's winning more often - white or black.
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static int ContestTwoRandoms(RandomPlayer random)
        {
            GameInstance.Reset();
            while (!GameInstance.CheckIfGameEnded())
            {
                var move = random.Move(GameInstance);
                GameInstance.Move(move);
            }
            return GameInstance.CheckWinner();
        }
    }
}
