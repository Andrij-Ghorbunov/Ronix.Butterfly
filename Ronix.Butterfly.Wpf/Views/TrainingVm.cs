using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ronix.Butterfly.Wpf.Backpropagation;
using Ronix.Framework.Mvvm;
using Ronix.Neural;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ronix.Butterfly.Wpf.Views
{
    public class TrainingVm : ViewModelBase
    {
        public event Action OnNewLogRecord;

        public EvolutionVm Evolution { get; set; }

        private string _lastNetworkDirectory, _lastDatasetDirectory;

        private bool _isFolder;

        public bool IsFolder
        {
            get => _isFolder;
            set => SetValue(ref _isFolder, value);
        }

        private string _networkPath;

        public string NetworkPath
        {
            get => _networkPath;
            set => SetValue(ref _networkPath, value);
        }


        private string _networkFolderPath;

        public string NetworkFolderPath
        {
            get => _networkFolderPath;
            set => SetValue(ref _networkFolderPath, value);
        }

        private string _datasetPath;

        public string DatasetPath
        {
            get => _datasetPath;
            set => SetValue(ref _datasetPath, value);
        }

        private bool _isArchitecture50;

        public bool IsArchitecture50
        {
            get => _isArchitecture50;
            set => SetValue(ref _isArchitecture50, value);
        }

        public void ApplySettings(AppSettings settings)
        {
            _lastNetworkDirectory = settings.RootFolder;
            _lastDatasetDirectory = settings.RootFolder;
        }

        private bool _isArchitecture1;

        public bool IsArchitecture1
        {
            get => _isArchitecture1;
            set => SetValue(ref _isArchitecture1, value);
        }

        private bool _canTrain;

        public bool CanTrain
        {
            get => _canTrain;
            set => SetValue(ref _canTrain, value);
        }

        private bool _isAutoSave;

        public bool IsAutoSave
        {
            get => _isAutoSave;
            set => SetValue(ref _isAutoSave, value);
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetValue(ref _isBusy, value, IsBusyChanged);
        }

        private void IsBusyChanged()
        {
            _stop.RaiseCanExecuteChanged();
        }

        public LayeredNeuralNetworkWithBackpropagation Network { get; private set; }

        public DatasetFile Dataset { get; private set; }

        public List<LayeredNeuralNetworkWithBackpropagation> Networks { get; private set; }

        public ObservableCollection<string> Log { get; }

        private readonly Command _save, _train;

        public ICommand BrowseNetwork { get; }

        public ICommand BrowseDataset { get; }

        public ICommand BrowseFolder { get; }

        public ICommand Save => _save;

        public ICommand Train => _train;


        private readonly Command _stop;

        public ICommand Stop => _stop;

        private void StopCommand()
        {
            _isStopRequested = true;
        }

        private bool StopCommandPossible()
        {
            return IsBusy;
        }

        public ICommand ClearLog { get; }

        #region Architecture 1

        private double _multiplierForScore = 10;

        public double MultiplierForScore
        {
            get => _multiplierForScore;
            set => SetValue(ref _multiplierForScore, value);
        }


        private double _multiplierForDevelopment = 1;

        public double MultiplierForDevelopment
        {
            get => _multiplierForDevelopment;
            set => SetValue(ref _multiplierForDevelopment, value);
        }

        private double _bonusForCorrectMove = 0.1;

        public double BonusForCorrectMove
        {
            get => _bonusForCorrectMove;
            set => SetValue(ref _bonusForCorrectMove, value);
        }

        private double _bonusForIncorrectMove = 0;

        public double BonusForIncorrectMove
        {
            get => _bonusForIncorrectMove;
            set => SetValue(ref _bonusForIncorrectMove, value);
        }

        private double _correction = 0.5;

        public double Correction
        {
            get => _correction;
            set => SetValue(ref _correction, value);
        }

        #endregion Architecture 1

        #region Architecture 50

        private double _valueForCorrectMove = 1;

        public double ValueForCorrectMove
        {
            get => _valueForCorrectMove;
            set => SetValue(ref _valueForCorrectMove, value);
        }

        private double _valueForIncorrectMove = 0;

        public double ValueForIncorrectMove
        {
            get => _valueForIncorrectMove;
            set => SetValue(ref _valueForIncorrectMove, value);
        }

        private double _valueForIllegalMove = 0;

        public double ValueForIllegalMove
        {
            get => _valueForIllegalMove;
            set => SetValue(ref _valueForIllegalMove, value);
        }

        private bool _hasValueForIllegalMove = false;

        public bool HasValueForIllegalMove
        {
            get => _hasValueForIllegalMove;
            set => SetValue(ref _hasValueForIllegalMove, value);
        }

        #endregion Architecture 50

        public TrainingSetupVm WinningSetup { get; }
        public TrainingSetupVm LosingSetup { get; }
        public TrainingSetupVm DrawSetup { get; }
        public DatasetInfoPanelVm DatasetInfo { get; }

        public TrainingVm()
        {
            BrowseDataset = new Command(BrowseDatasetCommand);
            BrowseNetwork = new Command(BrowseNetworkCommand);
            BrowseFolder = new Command(BrowseFolderCommand);
            _save = new Command(SaveCommand, SaveCommandPossible);
            _train = new Command(TrainCommand, TrainCommandPossible);
            _stop = new Command(StopCommand, StopCommandPossible);
            WinningSetup = new TrainingSetupVm(this, x => x.Winning) { Name = "Winning Moves", IsEnabled = true };
            LosingSetup = new TrainingSetupVm(this, x => x.Losing) { Name = "Losing Moves" };
            DrawSetup = new TrainingSetupVm(this, x => x.Draws) { Name = "Draw Moves" };
            DatasetInfo = new DatasetInfoPanelVm();
            Log = new ObservableCollection<string>();
            ClearLog = new Command(ClearLogCommand);
        }

        private void BrowseNetworkCommand()
        {
            var file = BrowseFile("Choose network to train", "Neural networks (*.nn)|*.nn", ref _lastNetworkDirectory);
            if (file == null) return;
            NetworkPath = file;
            Network = LayeredNeuralNetworkWithBackpropagation.LoadFromFile(file);
            if (Network.InputNumber != 38)
            {
                MessageBox.Show($"I don't know how to train a network with this number of input neurons: {Network.InputNumber} (must be 38)");
                NetworkPath = null;
                Network = null;
                Update();
                return;
            }
            IsArchitecture1 = Network.OutputNumber == 1;
            IsArchitecture50 = Network.OutputNumber == 50;
            if (!IsArchitecture1 && !IsArchitecture50)
            {
                MessageBox.Show($"I don't know how to train a network with this number of output neurons: {Network.OutputNumber} (must be either 1 or 50)");
                NetworkPath = null;
                Network = null;
                Update();
                return;
            }
            if (IsArchitecture50)
                Network.IsClassificator = true;
            IsFolder = false;
            Update();
        }

        private void BrowseFolderCommand()
        {
            var dlg = new CommonOpenFileDialog { IsFolderPicker = true, Title = "Select folder with neural networks", InitialDirectory = _lastNetworkDirectory };
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok) return;
            NetworkFolderPath = dlg.FileName;
            _lastNetworkDirectory = NetworkFolderPath;
            var files = Directory.EnumerateFiles(NetworkFolderPath, "*.nn").ToList();
            var is1 = false;
            var is50 = false;
            var list = new List<LayeredNeuralNetworkWithBackpropagation>();
            foreach (var file in files)
            {
                var network = LayeredNeuralNetworkWithBackpropagation.LoadFromFile(file);
                if (network.InputNumber != 38 || (network.OutputNumber != 1 && network.OutputNumber != 50))
                {
                    MessageBox.Show($"Network {network.FileName} contains wrong number of inputs or outputs, supported architectures are 38-1 and 38-50");
                    return;
                }
                if (network.OutputNumber == 50)
                {
                    is50 = true;
                    network.IsClassificator = true;
                }
                else
                {
                    is1 = true;
                }
                list.Add(network);
            }
            if (!list.Any())
            {
                MessageBox.Show("This folder contains no networks!");
                return;
            }
            Networks = list;
            IsArchitecture1 = is1;
            IsArchitecture50 = is50;
            IsFolder = true;
            Update();
        }

        private void BrowseDatasetCommand()
        {
            var file = BrowseFile("Choose dataset", "Datasets (*.dts)|*.dts", ref _lastDatasetDirectory);
            if (file == null) return;
            DatasetPath = file;
            Dataset = DatasetFile.Load(file);
            DatasetInfo.Load(Dataset);
            Update();
        }

        private string BrowseFile(string title, string filter, ref string last)
        {
            var d = new OpenFileDialog();
            d.Title = title;
            d.Filter = filter;
            d.Multiselect = false;
            if (!string.IsNullOrEmpty(last))
                d.InitialDirectory = last;
            if (d.ShowDialog() != true) return null;
            last = Path.GetDirectoryName(d.FileName);
            return d.FileName;
        }

        private void Update()
        {
            CanTrain = (IsFolder ? Networks.Any() : Network != null) && Dataset != null && !IsBusy;
            _save.RaiseCanExecuteChanged();
            _train.RaiseCanExecuteChanged();
        }

        private async void TrainCommand()
        {
            var setups = new[] { WinningSetup, DrawSetup, LosingSetup }.Where(it => it.IsEnabled).ToList();
            if (!setups.Any())
            {
                MessageBox.Show("No setups are selected!");
                return;
            }
            ResetProgress();
            IsBusy = true;
            Update();
            _isStopRequested = false;
            await Task.Factory.StartNew(() => ActivateAllSetups(setups));
            _isStopRequested = false;
            if (IsAutoSave)
                SaveCommand();
            ResetProgress();
            IsBusy = false;
            Update();
        }

        private void SaveCommand()
        {
            if (IsFolder)
            {
                foreach (var network in Networks)
                {
                    network.Save(Path.Combine(NetworkFolderPath, network.FileName));
                }
            }
            else
            {
                Network.Save(NetworkPath);
            }
        }

        private bool TrainCommandPossible()
        {
            return CanTrain;
        }

        private bool SaveCommandPossible()
        {
            return CanTrain;
        }

        private void ClearLogCommand()
        {
            Log.Clear();
        }

        #region Progress visualization

        private int _datasetItemCount;

        public int DatasetItemCount
        {
            get => _datasetItemCount;
            set => SetValue(ref _datasetItemCount, value);
        }

        private int _iterationNumber;

        public int IterationNumber
        {
            get => _iterationNumber;
            set => SetValue(ref _iterationNumber, value);
        }

        private int _iterationTotalNumber;

        public int IterationTotalNumber
        {
            get => _iterationTotalNumber;
            set => SetValue(ref _iterationTotalNumber, value);
        }

        private double _progress;

        public double Progress
        {
            get => _progress;
            set => SetValue(ref _progress, value);
        }

        private string _currentSetup;

        public string CurrentSetup
        {
            get => _currentSetup;
            set => SetValue(ref _currentSetup, value);
        }

        private void ResetProgress()
        {
            Progress = 0;
            DatasetItemCount = 0;
            IterationNumber = 0;
            CurrentSetup = string.Empty;
        }

        #endregion Progress visualization

        private Dataset PrepareDataset(List<DatasetItem> items)
        {
            if (IsArchitecture1)
            {
                return PrepareDataset1(items);
            }
            if (IsArchitecture50)
            {
                return PrepareDataset50(items);
            }
            throw new Exception("Unknown architecture");
        }

        public Dataset PrepareDataset1(List<DatasetItem> items)
        {
            return DatasetItemConverter.ConvertTo1(items.ToArray(),
                MultiplierForScore, MultiplierForDevelopment, BonusForCorrectMove, BonusForIncorrectMove, Correction);
        }

        public Dataset PrepareDataset50(List<DatasetItem> items)
        {
            double? valueForIllegalMove = null;
            if (HasValueForIllegalMove)
                valueForIllegalMove = ValueForIllegalMove;
            return DatasetItemConverter.ConvertTo50(items.ToArray(),
                ValueForCorrectMove, ValueForIncorrectMove, valueForIllegalMove);
        }

        private void ActivateAllSetups(List<TrainingSetupVm> setups)
        {
            foreach (var setup in setups)
            {
                if (_isStopRequested) return;
                ActivateSetup(setup);
            }
        }

        private bool _isStopRequested;

        private void ActivateSetup(TrainingSetupVm setup)
        {
            Dataset dataset = null;
            Dataset dataset1 = null;
            Dataset dataset50 = null;
            if (IsFolder)
            {
                dataset1 = PrepareDataset1(setup.ItemsSelector(Dataset));
                dataset50 = PrepareDataset50(setup.ItemsSelector(Dataset));
            }
            else
            {
                dataset = PrepareDataset(setup.ItemsSelector(Dataset));
            }

            var iterations = setup.NumberOfIterations;
            var learningRate = setup.LearningRate;
            int? maxBatchSize = null;
            if (setup.HasMaxBlock)
                maxBatchSize = setup.MaxBlock;
            CurrentSetup = setup.Name;

            IterationTotalNumber = iterations;
            var invIter = 1d / iterations;
            var smallInvIter = 0d;
            if (IsFolder) smallInvIter = invIter / Networks.Count;

            for (var iteration = 0; iteration < iterations; iteration++)
            {
                if (_isStopRequested) break;
                if (IsFolder)
                {
                    IterationNumber = iteration;
                    var index = 0;
                    foreach (var network in Networks)
                    {
                        if (_isStopRequested) break;
                        dataset = network.OutputNumber == 1 ? dataset1 : dataset50;
                        var stats = network.Train(dataset.Inputs, dataset.Outputs, learningRate, maxBatchSize);
                        var statsStr = network.FileName + ": " + stats.ToString();
                        Progress = iteration * invIter + (index++) * smallInvIter;
                        Application.Current.Dispatcher.BeginInvoke(() => AddLog(statsStr));
                    }
                }
                else
                {
                    var stats = Network.Train(dataset.Inputs, dataset.Outputs, learningRate, maxBatchSize);
                    var statsStr = Network.FileName + ": " + stats.ToString();
                    IterationNumber = iteration;
                    Progress = iteration * invIter;
                    Application.Current.Dispatcher.BeginInvoke(() => AddLog(statsStr));
                }
            }
        }

        private void AddLog(string message)
        {
            Log.Add(message);
            OnNewLogRecord?.Invoke();
        }
    }
}
