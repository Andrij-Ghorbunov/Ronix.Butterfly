using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly
{
    public class DatasetItem
    {
        public readonly byte[] InputData;

        public readonly int ScoreSum, ScoreDifference;

        public readonly int MoveSelected;

        public string Text { get; }

        public DatasetItem(byte[] inputData, int scoreSum, int scoreDifference, int moveSelected)
        {
            InputData = inputData;
            ScoreSum = scoreSum;
            ScoreDifference = scoreDifference;
            MoveSelected = moveSelected;

            var baseName = MoveSelected < 10 ? "Development" : MoveSelected < 22 ? "Advance" : MoveSelected < 26 ? "Score" : "Attack";
            var scoreA = (ScoreSum + ScoreDifference) / 2;
            var scoreB = (ScoreSum - ScoreDifference) / 2;
            Text = baseName == "Score" ? $"{baseName} ({scoreA}-{scoreA + 1}:{scoreB})" : $"{baseName} ({scoreA}:{scoreB})";
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
