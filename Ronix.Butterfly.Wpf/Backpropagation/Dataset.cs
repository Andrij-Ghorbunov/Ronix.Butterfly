using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Backpropagation
{
    public class Dataset
    {
        public double[,] Inputs;
        public double[,] Outputs;

        public int GetItemsCount()
        {
            return Inputs.GetLength(0);
        }
    }
}
