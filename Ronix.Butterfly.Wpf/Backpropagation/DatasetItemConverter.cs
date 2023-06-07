using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Backpropagation
{
    public static class DatasetItemConverter
    {
        public static Dataset ConvertTo50(DatasetItem[] items, double valueForCorrectMove = 1, double valueForIncorrectMoves = 0, double? valueForIllegalMoves = null)
        {
            var count = items.Length;
            var inputs = new double[count, 38];
            var outputs = new double[count, 50];
            for (var i = 0; i < count; i++)
            {
                var item = items[i];
                for (var j = 0; j < 36; j++)
                {
                    var piece = item.InputData[j];
                    inputs[i, j] = piece == ButterflyConstants.White ? 1 : piece == ButterflyConstants.Black ? -1 : 0;
                }
                inputs[i, 36] = item.ScoreDifference;
                inputs[i, 37] = item.ScoreSum;

                for (var j = 0; j < 50; j++)
                {
                    outputs[i, j] = j == item.MoveSelected ? valueForCorrectMove : valueForIncorrectMoves;
                }

                if (valueForIllegalMoves.HasValue)
                {
                    var v = valueForIllegalMoves.Value;
                    var game = new ButterflyGame();
                    Array.Copy(item.InputData, game.Field, 36);
                    //game.CheckIfGameEnded(); // fill possible moves array
                    for (var j = 0; j < 50; j++)
                    {
                        if (!game.IsMovePossible(j)) // actually, this method doesn't require filling the array
                            outputs[i, j] = v;
                    }
                }
            }
            return new Dataset { Inputs = inputs, Outputs = outputs };
        }
        
        public static Dataset ConvertTo1(DatasetItem[] items, double multiplierForScore = 10,
            double multiplierForDevelopment = 1, double bonusForCorrectMove = 0.1, double bonusForIncorrectMoves = 0, double correction = 0.5)
        {
            var count = items.Length;
            var convertedItems = new List<DatasetItemFor1>();
            var game = new ButterflyGame();
            for (var i = 0; i < count; i++)
            {
                var item = items[i];
                Array.Copy(item.InputData, game.Field, 36);

                var maxBasicTarget = double.NegativeInfinity;
                var maxTarget = double.NegativeInfinity;
                DatasetItemFor1 correctChoice = null;

                for (var j = 0; j < 50; j++)
                {
                    if (!game.IsMovePossible(j)) continue;
                    var copyGame = new ButterflyGame(game);
                    copyGame.Move(j);
                    var field = copyGame.Field;
                    var score = copyGame.ScoreWhite - copyGame.ScoreBlack;
                    var development = copyGame.GetDevelopment();
                    var basicTarget = multiplierForScore * score + multiplierForDevelopment * development;
                    var bonus = j == item.MoveSelected ? bonusForCorrectMove : bonusForIncorrectMoves;
                    var target = basicTarget + bonus;
                    var datasetItem = new DatasetItemFor1(field, item.ScoreSum, item.ScoreDifference, target);
                    convertedItems.Add(datasetItem);
                    if (basicTarget > maxBasicTarget)
                        maxBasicTarget = basicTarget;
                    if (target > maxTarget)
                        maxTarget = target;
                    if (j == item.MoveSelected)
                        correctChoice = datasetItem;
                }

                if (correctChoice == null) continue;

                if (correctChoice.Target < maxTarget)
                {
                    // so the trained network will actually select this move in this situation
                    correctChoice.Target = maxTarget + correction;
                }
            }
            var convCount = convertedItems.Count;
            var inputs = new double[convCount, 38];
            var outputs = new double[convCount, 1];
            for (var i = 0; i < convCount; i++)
            {
                var item = convertedItems[i];
                for (var j = 0; j < 36; j++)
                {
                    var piece = item.Field[j];
                    inputs[i, j] = piece == ButterflyConstants.White ? 1 : piece == ButterflyConstants.Black ? -1 : 0;
                }
                inputs[i, 36] = item.ScoreDiff;
                inputs[i, 37] = item.ScoreSum;
                outputs[i, 0] = item.Target;
            }

            return new Dataset { Inputs = inputs, Outputs = outputs };
        }
    }
}
