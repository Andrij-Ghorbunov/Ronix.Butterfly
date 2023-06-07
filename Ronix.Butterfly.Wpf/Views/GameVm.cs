using Microsoft.Win32;
using Ronix.Butterfly.Wpf.Players;
using Ronix.Framework.Mvvm;
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
    public class GameVm : ViewModelBase
    {
        public event Action DatasetsUpdated;

        private PlayerBase _whitePlayer, _blackPlayer;

        public PlayerBase WhitePlayer
        {
            get => _whitePlayer;
            set => SetValue(ref _whitePlayer, value, WhitePlayerChanged);
        }
        public PlayerBase BlackPlayer
        {
            get => _blackPlayer;
            set => SetValue(ref _blackPlayer, value, BlackPlayerChanged);
        }

        private bool _whiteEnabled, _blackEnabled;

        public bool WhiteEnabled
        {
            get => _whiteEnabled;
            set => SetValue(ref _whiteEnabled, value);
        }

        public bool BlackEnabled
        {
            get => _blackEnabled;
            set => SetValue(ref _blackEnabled, value);
        }

        private string _whitePath, _blackPath;

        public string WhitePath
        {
            get => _whitePath;
            set => SetValue(ref _whitePath, value);
        }

        public void ApplySettings(AppSettings settings)
        {
            _lastDirectory = settings.RootFolder;
        }

        public string BlackPath
        {
            get => _blackPath;
            set => SetValue(ref _blackPath, value);
        }

        public List<DatasetItem> WinnerMoves { get; }
        public List<DatasetItem> LoserMoves { get; }
        public List<DatasetItem> DrawMoves { get; }

        public ObservableCollection<PlayerBase> WhitePlayers { get; }
        public ObservableCollection<PlayerBase> BlackPlayers { get; }

        public ICommand BrowseWhite { get; }
        public ICommand BrowseBlack { get; }
        public ICommand SwapPlayers { get; }
        public ICommand Rules { get; }
        public ICommand RulesEng { get; }

        private void WhitePlayerChanged(PlayerBase newValue)
        {
            WhiteEnabled = newValue.NeedsFileBrowsing;
        }

        private void BlackPlayerChanged(PlayerBase newValue)
        {
            BlackEnabled = newValue.NeedsFileBrowsing;
        }

        public GameVm()
        {
            BrowseWhite = new Command(BrowseWhiteCommand);
            BrowseBlack = new Command(BrowseBlackCommand);
            SwapPlayers = new Command(SwapPlayersCommand);
            Rules = new Command(RulesCommand);
            RulesEng = new Command(RulesEngCommand);
            WhitePlayers = new ObservableCollection<PlayerBase>
            {
                new HumanPlayer(),
                new RandomPlayer(),
                new NeuralPlayer()
            };
            BlackPlayers = new ObservableCollection<PlayerBase>
            {
                new HumanPlayer(),
                new RandomPlayer(),
                new NeuralPlayer()
            };
            WhitePlayer = WhitePlayers[0];
            BlackPlayer = BlackPlayers[0];
            WinnerMoves = new List<DatasetItem>();
            LoserMoves = new List<DatasetItem>();
            DrawMoves = new List<DatasetItem>();
        }

        private string _lastDirectory;

        private string BrowseFile(string title)
        {
            var d = new OpenFileDialog();
            d.Title = title;
            d.Filter = "Neural networks (*.nn)|*.nn";
            d.Multiselect = false;
            if (!string.IsNullOrEmpty(_lastDirectory))
                d.InitialDirectory = _lastDirectory;
            if (d.ShowDialog() != true) return null;
            _lastDirectory = Path.GetDirectoryName(d.FileName);
            return d.FileName;
        }

        private void BrowseWhiteCommand()
        {
            var path = BrowseFile("Open network file for white player");
            if (path == null) return;
            WhitePath = path;
            WhitePlayer.Load(path);
        }

        private void BrowseBlackCommand()
        {
            var path = BrowseFile("Open network file for black player");
            if (path == null) return;
            BlackPath = path;
            BlackPlayer.Load(path);
        }

        private void SwapPlayersCommand()
        {
            var whiteName = WhitePlayer.Name;
            var blackName = BlackPlayer.Name;
            WhitePlayer = WhitePlayers.First(it => it.Name == blackName);
            BlackPlayer = BlackPlayers.First(it => it.Name == whiteName);
            var path = WhitePath;
            WhitePath = BlackPath;
            BlackPath = path;
            WhitePlayer.Load(WhitePath);
            BlackPlayer.Load(BlackPath);
        }

        private void RulesCommand()
        {
            MessageBox.Show(Helpers.Rules.RulesUkr, "Правила гри");
        }

        private void RulesEngCommand()
        {
            MessageBox.Show(Helpers.Rules.RulesEng, "Rules of the game");
        }

        #region Datasets

        private int _winnerMovesCount;

        public int WinnerMovesCount
        {
            get => _winnerMovesCount;
            set => SetValue(ref _winnerMovesCount, value);
        }

        private int _loserMovesCount;

        public int LoserMovesCount
        {
            get => _loserMovesCount;
            set => SetValue(ref _loserMovesCount, value);
        }

        private int _drawMovesCount;

        public int DrawMovesCount
        {
            get => _drawMovesCount;
            set => SetValue(ref _drawMovesCount, value);
        }


        private int _totalMovesCount;

        public int TotalMovesCount
        {
            get => _totalMovesCount;
            set => SetValue(ref _totalMovesCount, value);
        }


        public void UpdateDatasetsCounters()
        {
            WinnerMovesCount = WinnerMoves.Count;
            DrawMovesCount = DrawMoves.Count;
            LoserMovesCount = LoserMoves.Count;
            TotalMovesCount = WinnerMovesCount + DrawMovesCount + LoserMovesCount;
            DatasetsUpdated?.Invoke();
        }

        public void AddDatasetItems(List<DatasetItem> winnerMoves, List<DatasetItem> loserMoves, List<DatasetItem> drawMoves)
        {
            WinnerMoves.AddRange(winnerMoves);
            LoserMoves.AddRange(loserMoves);
            DrawMoves.AddRange(drawMoves);
            UpdateDatasetsCounters();
        }

        public void ClearDataset()
        {
            WinnerMoves.Clear();
            LoserMoves.Clear();
            DrawMoves.Clear();
            UpdateDatasetsCounters();
        }

        #endregion Datasets
    }
}
