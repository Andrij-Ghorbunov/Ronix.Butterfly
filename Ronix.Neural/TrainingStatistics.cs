using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Neural
{
    public class TrainingStatistics
    {
        public double? RateOfCorrectGuesses { get; set; }
        public double MeanSquareDeviation { get; set; }

        public override string ToString()
        {
            if (RateOfCorrectGuesses.HasValue)
            {
                return $"{RateOfCorrectGuesses.Value:p2} guesses, {MeanSquareDeviation} deviation";
            }
            return $"{MeanSquareDeviation} deviation";
        }
    }
}
