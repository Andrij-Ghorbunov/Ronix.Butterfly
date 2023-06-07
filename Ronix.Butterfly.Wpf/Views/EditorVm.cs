using Microsoft.WindowsAPICodePack.Dialogs;
using Ronix.Framework.Mvvm;
using Ronix.Neural;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ronix.Butterfly.Wpf.Views
{
    public class EditorVm : ViewModelBase
    {
        private string _lastDirectory;

        public event Action<int> ConnectomePartChanged;

        private string _path;

        public string Path
        {
            get => _path;
            set => SetValue(ref _path, value);
        }


        private int _editedLayerIndex;

        public int EditedLayerIndex
        {
            get => _editedLayerIndex;
            set => SetValue(ref _editedLayerIndex, value);
        }

        private bool _isChanged;

        public bool IsChanged
        {
            get => _isChanged;
            set => SetValue(ref _isChanged, value, IsChangedChanged);
        }

        private void IsChangedChanged()
        {
            _save.RaiseCanExecuteChanged();
        }

        public ICommand Browse { get; }


        private readonly Command _save, _saveAs;

        public ICommand Save => _save;
        public ICommand SaveAs => _saveAs;

        private void SaveCommand()
        {
            Network.Save(Path);
            IsChanged = false;
        }

        private void SaveAsCommand()
        {
            var dlg = new CommonSaveFileDialog { Title = "Where to save the file", DefaultExtension = ".nn", InitialDirectory = _lastDirectory };
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok) return;
            var path = dlg.FileName;
            Network.Save(path);
        }

        public void ApplySettings(AppSettings settings)
        {
            _lastDirectory = settings.RootFolder;
        }

        private bool SaveCommandPossible()
        {
            return Network != null && IsChanged;
        }

        private bool SaveAsCommandPossible()
        {
            return Network != null;
        }

        public LayeredNeuralNetwork Network { get; set; }

        public LayersConstructorVm LayersConstructor { get; }

        public ObservableCollection<SynapseRowVm> Synapses { get; }

        public EditorVm()
        {
            Browse = new Command(BrowseCommand);
            _save = new Command(SaveCommand, SaveCommandPossible);
            _saveAs = new Command(SaveAsCommand, SaveAsCommandPossible);
            LayersConstructor = new LayersConstructorVm { AreSynapseEditable = true };
            LayersConstructor.Clear();
            Synapses = new ObservableCollection<SynapseRowVm>();
            LayersConstructor.OnEditSynapses += OnEditSynapses;
            LayersConstructor.OnChanged += OnEditLayers;
        }

        private void OnEditLayers()
        {
            var editorLayers = LayersConstructor.GetLayers();
            for (var layerIndex = 1; layerIndex < Network.Layers.Length; layerIndex++)
            {
                var origLayer = Network.Layers[layerIndex];
                var editorLayer = editorLayers[layerIndex];
                origLayer.ActivationFunction = editorLayer.ActivationFunction;
            }
            IsChanged = true;
        }

        private void OnEditSynapses(int index)
        {
            EditedLayerIndex = index;
            ConstructSynapseVms();
        }

        private void OnUneditSynapses()
        {
            EditedLayerIndex = -1;
            Synapses.Clear();
            ConnectomePartChanged?.Invoke(0);
        }

        private void BrowseCommand()
        {
            var dlg = new CommonOpenFileDialog { Title = "Select neural network file", DefaultExtension = ".nn", InitialDirectory = _lastDirectory };
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok) return;
            Path = dlg.FileName;
            _lastDirectory = System.IO.Path.GetDirectoryName(Path);
            Network = LayeredNeuralNetwork.LoadFromFile(Path);
            LayersConstructor.Layers.Clear();
            var isFirst = true;
            foreach (var layer in Network.Layers)
            {
                LayersConstructor.AddLayer(layer, !isFirst);
                isFirst = false;
            }
            OnUneditSynapses();
            LoadParameters();
            IsChanged = false;
            
            _save.RaiseCanExecuteChanged();
            _saveAs.RaiseCanExecuteChanged();
        }

        public double GetSynapse(int x, int y)
        {
            if (x == -1)
                return Network.Bias[EditedLayerIndex][y];
            return Network.Connectome[EditedLayerIndex][x, y];
        }

        public void SetSynapse(int x, int y, double newValue)
        {
            IsChanged = true;
            if (x == -1)
            {
                Network.Bias[EditedLayerIndex][y] = newValue;
            }
            else
            {
                Network.Connectome[EditedLayerIndex][x, y] = newValue;
            }
        }

        private void ConstructSynapseVms()
        {
            var rows = Network.Layers[EditedLayerIndex + 1].NumberOfNeurons;
            var columns = Network.Layers[EditedLayerIndex].NumberOfNeurons;
            Synapses.Clear();
            for (var row = 0; row < rows; row++)
            {
                Synapses.Add(new SynapseRowVm(this, row, columns));
            }
            ConnectomePartChanged?.Invoke(columns);
        }

        #region Parameters

        private int _totalGenerationNumber;

        public int TotalGenerationNumber
        {
            get => _totalGenerationNumber;
            set => SetValue(ref _totalGenerationNumber, value, OnTotalGenerationNumberChanged);
        }

        private void OnTotalGenerationNumberChanged(int newValue)
        {
            if (Network == null)
            {
                TotalGenerationNumber = 0;
                return;
            }
            Network.TotalGenerationNumber = newValue;
            IsChanged = true;
        }

        private int _generationNumber;

        public int GenerationNumber
        {
            get => _generationNumber;
            set => SetValue(ref _generationNumber, value, OnGenerationNumberChanged);
        }

        private void OnGenerationNumberChanged(int newValue)
        {
            if (Network == null)
            {
                GenerationNumber = 0;
                return;
            }
            Network.GenerationNumber = newValue;
            IsChanged = true;
        }

        private int _survivedGenerations;

        public int SurvivedGenerations
        {
            get => _survivedGenerations;
            set => SetValue(ref _survivedGenerations, value, OnSurvivedGenerationsChanged);
        }

        private void OnSurvivedGenerationsChanged(int newValue)
        {
            if (Network == null)
            {
                SurvivedGenerations = 0;
                return;
            }
            Network.GenerationNumber = newValue;
            IsChanged = true;
        }


        private string _lastEvolutionHistory;

        public string LastEvolutionHistory
        {
            get => _lastEvolutionHistory;
            set => SetValue(ref _lastEvolutionHistory, value, OnLastEvolutionHistoryChanged);
        }

        private void OnLastEvolutionHistoryChanged(string newValue)
        {
            if (Network == null)
            {
                LastEvolutionHistory = string.Empty;
                return;
            }
            Network.LastEvolutionHistory = newValue;
            IsChanged = true;
        }

        private string _comments;

        public string Comments
        {
            get => _comments;
            set => SetValue(ref _comments, value, OnCommentsChanged);
        }

        private void OnCommentsChanged(string newValue)
        {
            if (Network == null)
            {
                Comments = string.Empty;
                return;
            }
            Network.Comments = newValue;
            IsChanged = true;
        }

        private void LoadParameters()
        {
            TotalGenerationNumber = Network.TotalGenerationNumber;
            GenerationNumber = Network.GenerationNumber;
            SurvivedGenerations = Network.SurvivedGenerations;
            LastEvolutionHistory = Network.LastEvolutionHistory;
            Comments = Network.Comments;
        }

        #endregion Parameters

    }
}
