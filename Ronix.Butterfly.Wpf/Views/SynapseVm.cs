using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Views
{
    public class SynapseVm : ViewModelBase
    {
        private double _value;

        public double Value
        {
            get => _value;
            set => SetValue(ref _value, value, OnValueChanged);
        }

        public EditorVm Parent { get; }

        public int X { get; }

        public int Y { get; }

        public SynapseVm(EditorVm parent, int x, int y)
        {
            Parent = parent;
            X = x;
            Y = y;
            _value = Parent.GetSynapse(x, y);
        }

        private void OnValueChanged(double newValue)
        {
            Parent.SetSynapse(X, Y, newValue);
        }
    }
}
