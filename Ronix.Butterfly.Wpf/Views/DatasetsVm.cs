using Microsoft.Win32;
using Ronix.Butterfly.Wpf.Backpropagation;
using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ronix.Butterfly.Wpf.Views
{
    public class DatasetsVm: ViewModelBase
    {
        private string _lastDirectory;

        public GameVm Game { get; private set; }

        public DatasetFile DatasetFile { get; private set; }

        private string _countersText;

        public string CountersText
        {
            get => _countersText;
            set => SetValue(ref _countersText, value);
        }

        private string _fileCountersText;

        public string FileCountersText
        {
            get => _fileCountersText;
            set => SetValue(ref _fileCountersText, value);
        }

        private string _path;

        public string Path
        {
            get => _path;
            set => SetValue(ref _path, value);
        }

        private int _winningItemsCount;

        public int WinningItemsCount
        {
            get => _winningItemsCount;
            set => SetValue(ref _winningItemsCount, value);
        }

        private int _losingItemsCount;

        public int LosingItemsCount
        {
            get => _losingItemsCount;
            set => SetValue(ref _losingItemsCount, value);
        }

        private int _drawItemsCount;

        public void ApplySettings(AppSettings settings)
        {
            _lastDirectory = settings.RootFolder;
        }

        public int DrawItemsCount
        {
            get => _drawItemsCount;
            set => SetValue(ref _drawItemsCount, value);
        }

        private int _totalItemsCount;

        public int TotalItemsCount
        {
            get => _totalItemsCount;
            set => SetValue(ref _totalItemsCount, value);
        }

        private bool _isAutoSave;

        public bool IsAutoSave
        {
            get => _isAutoSave;
            set => SetValue(ref _isAutoSave, value);
        }

        private bool _isFileChosen;

        public bool IsFileChosen
        {
            get => _isFileChosen;
            set => SetValue(ref _isFileChosen, value, IsFileChosenChanged);
        }

        private void IsFileChosenChanged()
        {
            _append.RaiseCanExecuteChanged();
        }

        public ICommand Browse { get; }

        public ICommand Merge { get; }

        private readonly Command _append;

        public ICommand Append => _append;

        public SingleDatasetVm WinnerVm { get; }
        public SingleDatasetVm LoserVm { get; }
        public SingleDatasetVm  DrawVm { get; }

        private void AppendCommand()
        {
            var dataset = File.Exists(Path) ? DatasetFile.Load(Path) : new DatasetFile();
            dataset.Winning.AddRange(Game.WinnerMoves);
            dataset.Losing.AddRange(Game.LoserMoves);
            dataset.Draws.AddRange(Game.DrawMoves);
            dataset.Save(Path);
            DatasetFile = dataset;
            Game.ClearDataset();
            UpdateVms();
            UpdateCounters();
        }

        public void Save()
        {
            DatasetFile.Save(Path);
            UpdateCounters();
        }

        private bool AppendCommandPossible()
        {
            return IsFileChosen;
        }

        public DatasetsVm()
        {
            Merge = new Command(MergeCommand);
            Browse = new Command(BrowseCommand);
            _append = new Command(AppendCommand, AppendCommandPossible);
            WinnerVm = new SingleDatasetVm(this);
            LoserVm = new SingleDatasetVm(this);
            DrawVm = new SingleDatasetVm(this);
        }

        private void BrowseCommand()
        {
            var dialog = new SaveFileDialog { DefaultExt = ".dts", AddExtension = true, CheckFileExists = false, OverwritePrompt = false, InitialDirectory = _lastDirectory };
            if (dialog.ShowDialog() != true) return;
            Path = dialog.FileName;
            _lastDirectory = System.IO.Path.GetDirectoryName(Path);
            IsFileChosen = true;
            DatasetFile = File.Exists(Path) ? DatasetFile.Load(Path) : new DatasetFile();
            UpdateVms();
            UpdateCounters();
        }

        public void SetGame(GameVm game)
        {
            Game = game;
            game.DatasetsUpdated += UpdateProperties;
            UpdateProperties();
        }

        private void UpdateVms()
        {
            WinnerVm.Init(DatasetFile.Winning);
            LoserVm.Init(DatasetFile.Losing);
            DrawVm.Init(DatasetFile.Draws);
        }

        private void UpdateProperties()
        {
            CountersText = $"The Game tab currently has {Game.TotalMovesCount} moves stored, including {Game.WinnerMovesCount} winning, {Game.LoserMovesCount} losing, and {Game.DrawMovesCount} draw.";
            if (IsAutoSave && IsFileChosen)
            {
                IsAutoSave = false;
                AppendCommand();
                IsAutoSave = true;
            }
        }

        private void UpdateCounters()
        {
            if (DatasetFile == null) return;
            WinningItemsCount = DatasetFile.Winning.Count;
            LosingItemsCount = DatasetFile.Losing.Count;
            DrawItemsCount = DatasetFile.Draws.Count;
            TotalItemsCount = WinningItemsCount + LosingItemsCount + DrawItemsCount;
            FileCountersText = $"The dataset file contains {TotalItemsCount} moves, including {WinningItemsCount} winning, {LosingItemsCount} losing, and {DrawItemsCount} draw.";
        }

        private void MergeCommand()
        {
            var dialog = new OpenFileDialog { DefaultExt = ".dts", AddExtension = true, Multiselect = true, InitialDirectory = _lastDirectory, Title = "Select all files to merge" };
            if (dialog.ShowDialog() != true) return;
            var fileNames = dialog.FileNames;
            if (fileNames == null || fileNames.Length < 2)
            {
                MessageBox.Show("You have to select more than one .dts file!");
                return;
            }
            var r = new DatasetFile();
            foreach (var file in fileNames)
            {
                var dts = DatasetFile.Load(file);
                r.Winning.AddRange(dts.Winning);
                r.Losing.AddRange(dts.Losing);
                r.Draws.AddRange(dts.Draws);
            }

            var dialog2 = new SaveFileDialog { DefaultExt = ".dts", AddExtension = true, CheckFileExists = false,
                OverwritePrompt = true, InitialDirectory = _lastDirectory, Title = "Where to save the merged dataset" };
            if (dialog2.ShowDialog() != true) return;
            r.Save(dialog2.FileName);
        }
    }
}
