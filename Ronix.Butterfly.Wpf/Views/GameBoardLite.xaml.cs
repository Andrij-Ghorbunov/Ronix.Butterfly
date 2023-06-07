using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for GameBoardLite.xaml
    /// </summary>
    public partial class GameBoardLite : UserControl, IDisposable
    {
        public GameBoardLite()
        {
            InitializeComponent();

            var white = Brushes.White;
            var black = Brushes.Black;
            var whiteBattlefield = Brushes.LightCyan;
            var blackBattlefield = Brushes.DarkBlue;

            BlackPieceImage = LoadImage(Assembly.GetExecutingAssembly().GetManifestResourceStream("Ronix.Butterfly.Wpf.Images.black.png"));
            WhitePieceImage = LoadImage(Assembly.GetExecutingAssembly().GetManifestResourceStream("Ronix.Butterfly.Wpf.Images.white.png"));

            Width = 8 * CellWidth;
            Height = 8 * CellHeight;

            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var cell = new Rectangle();
                    cell.Width = CellWidth;
                    cell.Height = CellHeight;
                    cell.Fill = (i > 3 && j < 4)
                        ? (((i + j) % 2 == 0) ? whiteBattlefield : blackBattlefield)
                        : (((i + j) % 2 == 0) ? white : black);
                    Canvas.SetLeft(cell, CellWidth * i);
                    Canvas.SetTop(cell, CellHeight * j);
                    BoardCanvas.Children.Add(cell);
                }
            }

            _timer = new Timer(20);
            _timer.AutoReset = true;
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        public const int CellWidth = 50;
        public const int CellHeight = 50;

        private readonly List<GamePieceVisualLite> Pieces = new();

        private readonly ImageSource BlackPieceImage, WhitePieceImage;

        public void RemovePiece(GamePieceVisualLite gamePieceVisual)
        {
            Pieces.Remove(gamePieceVisual);
        }

        private readonly Timer _timer;

        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(Tick);
        }

        private ImageSource LoadImage(Stream stream)
        {
            var imageSource = new BitmapImage { CacheOption = BitmapCacheOption.OnLoad };
            imageSource.BeginInit();
            imageSource.StreamSource = stream;
            imageSource.EndInit();
            return imageSource;
        }

        public void Clear()
        {
            foreach (var piece in Pieces.ToList())
            {
                piece.Remove();
            }
            Pieces.Clear();
        }

        public void InitMove(DatasetItem item)
        {
            Clear();
            if (item == null) return;
            var gameSide = !(DataContext as SingleDatasetVm).IsInverted;

            var fromCell = ButterflyGame.Moves[4 * item.MoveSelected + (gameSide ? 0 : 2)];
            var toCell = ButterflyGame.Moves[4 * item.MoveSelected + (gameSide ? 0 : 2) + 1];

            for (var cell = 0; cell < 36; cell++)
            {
                var input = gameSide ? item.InputData[cell] : item.InputData[ButterflyConstants.Flip[cell]];
                if (input == 0) continue;
                var side = gameSide == (input == ButterflyConstants.White);
                var x = ButterflyGameVisualizer.OptimizedToVisual[cell, 0];
                var y = ButterflyGameVisualizer.OptimizedToVisual[cell, 1];
                var vis = new Image
                {
                    Width = CellWidth,
                    Height = CellHeight,
                    Source = side ? WhitePieceImage : BlackPieceImage,
                };
                BoardCanvas.Children.Add(vis);
                var pv = new GamePieceVisualLite(x, y, side, vis, BoardCanvas, this);
                Pieces.Add(pv);
                pv.SetExactPosition();

                if (cell == toCell || (cell == fromCell && toCell == 255))
                {
                    pv.IsRemoved = true;
                }
                if (cell == fromCell && toCell != 255)
                {
                    var newX = ButterflyGameVisualizer.OptimizedToVisual[toCell, 0];
                    var newY = ButterflyGameVisualizer.OptimizedToVisual[toCell, 1];
                    pv.X = newX;
                    pv.Y = newY;
                }

                pv.Update();
            }
        }

        private void Tick()
        {
            foreach (var piece in Pieces.ToList())
            {
                piece.Tick();
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Elapsed -= TimerElapsed;
        }

        private void OnBoardDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is SingleDatasetVm oldValue)
                oldValue.OnSelectionChanged -= OnSelectionChanged;
            if (e.NewValue is SingleDatasetVm newValue)
                newValue.OnSelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(DatasetItem item)
        {
            InitMove(item);
        }

    }
}
