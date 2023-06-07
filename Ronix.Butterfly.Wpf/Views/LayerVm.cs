using Ronix.Framework.Mvvm;
using Ronix.Neural.Activation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Views
{
    public class LayerVm : ViewModelBase
    {
        public LayersConstructorVm Parent { get; }

        private int _number;

        public int Number
        {
            get => _number;
            set => SetValue(ref _number, value, ValidateNumber);
        }

        private ActivationFunctionBase _activation;

        public ActivationFunctionBase Activation
        {
            get => _activation;
            set => SetValue(ref _activation, value, ActivationFunctionChanged);
        }

        private void ActivationFunctionChanged()
        {
            Parent.Changed();
        }

        public ObservableCollection<ActivationFunctionBase> Activations => Parent.Activations;

        public bool IsRemovable { get; set; } = true;
        public bool IsNumberEditable { get; set; } = true;
        public bool IsActivationEditable { get; set; } = true;
        public bool IsInsertableAfter { get; set; } = true;
        public bool IsSynapseEditable { get; set; } = false;

        private void ValidateNumber(int oldValue, int newValue)
        {
            if (newValue <= 0)
                Number = oldValue <= 0 ? 42 : oldValue;
            Parent.UpdateNumberOfParameters();
        }

        public LayerVm(LayersConstructorVm parent, ActivationFunctionBase activation)
        {
            _activation = activation;
            Parent = parent;
        }
    }
}
