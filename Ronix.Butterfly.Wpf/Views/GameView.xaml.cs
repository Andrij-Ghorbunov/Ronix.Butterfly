using Ronix.Butterfly.Wpf.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ronix.Butterfly.Wpf.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        private ButterflyGame _currentGame;

        private ButterflyGameVisualizer _currentGameVisualizer;

        public GameView()
        {
            InitializeComponent();

            _currentGame = new ButterflyGame();
            _currentGameVisualizer = new ButterflyGameVisualizer(_currentGame);

            Board.InitGame(_currentGameVisualizer);
            Board.Updated += Update;
        }

        public void Update()
        {
            ScoreWhite.Text = _currentGame.ScoreWhite.ToString();
            ScoreBlack.Text = _currentGame.ScoreBlack.ToString();
            DevelopmentWhite.Text = _currentGameVisualizer.GetDevelopmentWhite().ToString();
            DevelopmentBlack.Text = _currentGameVisualizer.GetDevelopmentBlack().ToString();
            MoveMarker.Text = _currentGame.MoveSide ? "Move: white" : "Move: black";

            var isGameEnded = _currentGame.CheckIfGameEnded();
            if (isGameEnded)
            {
                Board.EndGame();
                var winner = _currentGame.CheckWinner();
                SaveDatasetItems(winner);
                var msg = winner > 0 ? "White has won!" : (winner < 0 ? "Black has won!" : "Draw!");
                MessageBox.Show(msg);
            }
        }

        private void SaveDatasetItems(int winner)
        {
            var whiteMoves = Board.WhitePlayer.ContributesToDataset ? _currentGameVisualizer.WhiteMoves : new List<DatasetItem>();
            var blackMoves = Board.BlackPlayer.ContributesToDataset ? _currentGameVisualizer.BlackMoves : new List<DatasetItem>();
            var winnerMoves = winner > 0 ? whiteMoves : winner < 0 ? blackMoves : new List<DatasetItem>();
            var loserMoves = winner < 0 ? whiteMoves : winner > 0 ? blackMoves : new List<DatasetItem>();
            var drawMoves = winner == 0 ? whiteMoves.Concat(blackMoves).ToList() : new List<DatasetItem>();
            if (DataContext is GameVm vm)
            {
                vm.AddDatasetItems(winnerMoves, loserMoves, drawMoves);
            }
        }

        private void StartNewGame(object sender, RoutedEventArgs e)
        {
            var vm = (GameVm)DataContext;
            var whitePlayer = vm.WhitePlayer.Copy();
            var blackPlayer = vm.BlackPlayer.Copy();
            if (whitePlayer is NeuralPlayer w && w.Network == null)
            {
                MessageBox.Show("You didn't select file for white player!");
                return;
            }
            if (blackPlayer is NeuralPlayer b && b.Network == null)
            {
                MessageBox.Show("You didn't select file for black player!");
                return;
            }
            PlayerMarkerWhite.Text = whitePlayer.Description;
            PlayerMarkerBlack.Text = blackPlayer.Description;
            Board.WhitePlayer = whitePlayer;
            Board.BlackPlayer = blackPlayer;
            _currentGame = new ButterflyGame();
            _currentGameVisualizer = new ButterflyGameVisualizer(_currentGame);
            Board.InitGame(_currentGameVisualizer);
        }
    }
}
