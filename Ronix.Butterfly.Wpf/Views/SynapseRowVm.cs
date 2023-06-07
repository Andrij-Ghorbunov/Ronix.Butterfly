using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Views
{
    public class SynapseRowVm : ViewModelBase
    {
        public EditorVm Parent { get; }

        public int Y { get; }

        public SynapseVm Bias { get; }

        public SynapseVm[] Data { get; }

        public int Number { get; }

        public SynapseRowVm(EditorVm parent, int y, int numberOfSynapses)
        {
            Parent = parent;
            Y = y;
            Number = y + 1;
            Bias = new SynapseVm(parent, -1, y);
            Data = new SynapseVm[numberOfSynapses];
            for (var index = 0; index < numberOfSynapses; index++)
            {
                Data[index] = new SynapseVm(parent, index, y);
            }
        }
    }
}
