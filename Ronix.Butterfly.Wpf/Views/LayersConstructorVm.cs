using Ronix.Butterfly.Wpf.Evolution;
using Ronix.Framework.Mvvm;
using Ronix.Neural;
using Ronix.Neural.Activation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ronix.Butterfly.Wpf.Views
{
    public class LayersConstructorVm : ViewModelBase
    {
        public event Action<int> OnEditSynapses;
        public event Action OnChanged;
        public ObservableCollection<LayerVm> Layers { get; }
        public ObservableCollection<ActivationFunctionBase> Activations { get; }
        public ICommand InsertAfter { get; }
        public ICommand Remove { get; }
        public ICommand EditSynapses { get; }


        private int _numberOfParameters;

        public int NumberOfParameters
        {
            get => _numberOfParameters;
            set => SetValue(ref _numberOfParameters, value);
        }

        private void EditSynapsesCommand(LayerVm arg)
        {
            var index = Layers.IndexOf(arg) - 1;
            OnEditSynapses?.Invoke(index);
        }

        public void Changed()
        {
            OnChanged?.Invoke();
        }

        public void UpdateNumberOfParameters()
        {
            if (Layers == null) return;
            var n = 0;
            for (var index = 0; index < Layers.Count - 1; index++)
            {
                n += (Layers[index].Number + 1) * Layers[index + 1].Number;
            }
            NumberOfParameters = n;
        }

        private bool _areSynapseEditable;

        public bool AreSynapseEditable
        {
            get => _areSynapseEditable;
            set => SetValue(ref _areSynapseEditable, value);
        }

        public LayersConstructorVm()
        {
            Layers = new ObservableCollection<LayerVm>
            {
                new LayerVm(this, ActivationFunctions.Null) { Number = Constants.InputNeurons, IsRemovable = false, IsNumberEditable = false, IsActivationEditable = false },
                new LayerVm(this, ActivationFunctions.Tanh) { Number = 60 },
                new LayerVm(this, ActivationFunctions.Tanh) { Number = 50 },
                new LayerVm(this, ActivationFunctions.Tanh) { Number = Constants.OutputNeurons, IsRemovable = false, IsInsertableAfter = false }
            };
            Activations = new ObservableCollection<ActivationFunctionBase>(ActivationFunctions.GetAll());
            InsertAfter = new Command<LayerVm>(InsertAfterCommand);
            EditSynapses = new Command<LayerVm>(EditSynapsesCommand);
            Remove = new Command<LayerVm>(RemoveCommand);
            UpdateNumberOfParameters();
        }

        public void Clear()
        {
            Layers.Clear();
            UpdateNumberOfParameters();
        }

        private void InsertAfterCommand(LayerVm layer)
        {
            Layers.Insert(Layers.IndexOf(layer) + 1, new LayerVm(this, ActivationFunctions.Linear) { Number = 10 });
        }

        public void AddLayer(LayerOfNeurons layer, bool activationEditable)
        {
            Layers.Add(new LayerVm(this, layer.ActivationFunction)
            {
                IsActivationEditable = activationEditable,
                IsSynapseEditable = activationEditable,
                IsInsertableAfter = false,
                IsNumberEditable = false,
                IsRemovable = false,
                Number = layer.NumberOfNeurons,
            });
            UpdateNumberOfParameters();
        }

        private void RemoveCommand(LayerVm layer)
        {
            Layers.Remove(layer);
            UpdateNumberOfParameters();
        }

        public LayerOfNeurons[] GetLayers()
        {
            return Layers.Select(it => new LayerOfNeurons(it.Number, it.Activation)).ToArray();
        }
    }
}
