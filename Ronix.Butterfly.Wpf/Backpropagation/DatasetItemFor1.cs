using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Backpropagation
{
    public class DatasetItemFor1
    {
        public byte[] Field { get; }

        public double Target { get; set; }

        public int ScoreSum { get; }

        public int ScoreDiff { get; }

        public DatasetItemFor1(byte[] field, int scoreSum, int scoreDiff, double target)
        {
            Field = field;
            Target = target;
            ScoreSum = scoreSum;
            ScoreDiff = scoreDiff;
        }
    }
}
