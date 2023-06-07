using Ronix.Butterfly.Wpf.Players;
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
    /// Interaction logic for GameBoardView.xaml
    /// </summary>
    public partial class GameBoardView : UserControl, IDisposable
    {
        public const int CellWidth = 50;
        public const int CellHeight = 50;

        private const int ScoreHighlightX = (int)(1.5 * CellWidth);
        private const int ScoreHighlightY = (int)(5.5 * CellWidth);

        private readonly List<GamePieceVisual> Pieces = new();

        private readonly List<GamePieceVisual> AttackTargets = new();

        private readonly ImageSource BlackPieceImage, WhitePieceImage;

        private readonly Rectangle SelectionHighlight;

        private readonly List<Rectangle> MoveHighlights;

        public bool IsGamePlaying { get; private set; }

        public void RemovePiece(GamePieceVisual gamePieceVisual)
        {
            Pieces.Remove(gamePieceVisual);
        }

        private ButterflyGameVisualizer Game;

        public event Action Updated;

        private readonly Timer _timer;

        private bool _isPromptRequested;

        public bool IsAnimationPlaying { get; private set; }

        public void EndGame()
        {
            _isPromptRequested = false;
            IsGamePlaying = false;
        }

        public PlayerBase WhitePlayer { get; set; } = new HumanPlayer();

        public PlayerBase BlackPlayer { get; set; } = new HumanPlayer();

        public Brush ScoreHighlightBrush = new RadialGradientBrush { GradientStops = { new GradientStop(Colors.White, 0.3), new GradientStop(Colors.Red, 1) } };

        public GameBoardView()
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

            SelectionHighlight = new Rectangle { Width = CellWidth, Height = CellHeight };
            SelectionHighlight.Fill = Brushes.Yellow;
            SelectionHighlight.Visibility = Visibility.Collapsed;
            BoardCanvas.Children.Add(SelectionHighlight);

            MoveHighlights = new List<Rectangle>();
            for (var i = 0; i < 3; i++)
            {
                var moveHighlight = new Rectangle { Width = CellWidth, Height = CellHeight };
                moveHighlight.Fill = Brushes.Pink;
                moveHighlight.Visibility = Visibility.Collapsed;
                MoveHighlights.Add(moveHighlight);
                BoardCanvas.Children.Add(moveHighlight);
                moveHighlight.MouseDown += MoveHighlightMouseDown;
            }

            _timer = new Timer(20);
            _timer.AutoReset = true;
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        public void AnimationStarted()
        {
            IsAnimationPlaying = Pieces.Any(it => it.IsAnimationPlayed);
        }

        public void AnimationEnded()
        {
            IsAnimationPlaying = Pieces.Any(it => it.IsAnimationPlayed);
            if (!IsAnimationPlaying && _isPromptRequested)
                PromptPlayer();
        }

        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(Tick);
        }

        private void MoveHighlightMouseDown(object sender, MouseButtonEventArgs e)
        {
            var move = ((Rectangle)sender).Tag as PossibleMove;
            if (move == null) return;
            if (SelectedPiece == null) return;
            var piece = SelectedPiece;
            var target = Pieces.FirstOrDefault(it => it.Piece.X == move.X && it.Piece.Y == move.Y);
            DeselectPiece();
            Game.Move(piece.Piece, move);
            piece.Update();
            target?.Update();
            Update();
        }

        private GamePieceVisual SelectedPiece;

        public void ClickPiece(GamePieceVisual gamePieceVisual)
        {
            if (!IsGamePlaying) return;
            if (AttackTargets.Contains(gamePieceVisual))
            {
                var selected = SelectedPiece;
                var target = gamePieceVisual;
                DeselectPiece();
                Game.MoveAttack(selected.Piece, target.Piece);
                selected.Update();
                target.Update();
                Update();
                return;
            }
            SelectPiece(gamePieceVisual);
        }

        private void DeselectPiece()
        {
            foreach (var highlight in MoveHighlights)
            {
                highlight.Visibility = Visibility.Collapsed;
            }
            AttackTargets.Clear();
            SelectionHighlight.Visibility = Visibility.Collapsed;
            SelectedPiece = null;
        }

        private void SelectPiece(GamePieceVisual gamePieceVisual)
        {
            if (gamePieceVisual.Piece.Side != Game.Game.MoveSide) return;
            Canvas.SetTop(SelectionHighlight, CellHeight * gamePieceVisual.Piece.X);
            Canvas.SetLeft(SelectionHighlight, CellWidth * gamePieceVisual.Piece.Y);
            SelectionHighlight.Visibility = Visibility.Visible;
            var possibleMoves = Game.GetPossibleMoves(gamePieceVisual.Piece).ToList();
            foreach (var highlight in MoveHighlights)
            {
                highlight.Visibility = Visibility.Collapsed;
            }
            AttackTargets.Clear();
            var index = 0;
            foreach (var move in possibleMoves)
            {
                var highlight = MoveHighlights[index++];
                Canvas.SetTop(highlight, move.IsScore ? ScoreHighlightY : CellHeight * move.X);
                Canvas.SetLeft(highlight, move.IsScore ? ScoreHighlightX : CellWidth * move.Y);
                highlight.Fill = move.IsScore ? ScoreHighlightBrush : move.IsAttack ? Brushes.Orange : Brushes.Pink;
                highlight.Visibility = Visibility.Visible;
                if (move.IsAttack) AttackTargets.Add(Pieces.First(it => it.Piece.X == move.X && it.Piece.Y == move.Y));
                highlight.Tag = move;
            }
            SelectedPiece = gamePieceVisual;
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

        private void EndAnimations()
        {
            foreach (var piece in Pieces.Where(it => it.IsAnimationPlayed).ToList())
            {
                piece.EndAnimation();
            }
        }

        public void InitGame(ButterflyGameVisualizer game)
        {
            if (IsAnimationPlaying)
            {
                _isPromptRequested = false;
                EndAnimations();
            }
            Clear();
            Game = game;
            foreach (var piece in game.Pieces)
            {
                var vis = new Image
                {
                    Width = CellWidth,
                    Height = CellHeight,
                    Source = piece.Side ? WhitePieceImage : BlackPieceImage,
                };
                BoardCanvas.Children.Add(vis);
                var pv = new GamePieceVisual(piece, vis, BoardCanvas, this);
                Pieces.Add(pv);
                pv.SetExactPosition();
            }
            IsGamePlaying = true;
            Game.Game.CheckIfGameEnded(); // initializes possible moves array
            PromptPlayer();
        }

        private void PromptPlayer()
        {
            if (!IsGamePlaying) return;
            if (IsAnimationPlaying)
            {
                _isPromptRequested = true;
                return;
            }
            _isPromptRequested = false;
            var player = Game.Game.MoveSide ? WhitePlayer : BlackPlayer;
            if (player == null) return;
            var index = player.Prompt(Game.Game);
            if (!index.HasValue) return;
            MoveByIndex(index.Value);
        }

        public void MoveByIndex(int index)
        {
            var move = Game.GetMoveFromIndex(index);
            var source = Pieces.FirstOrDefault(it => it.Piece.X == move.FromX && it.Piece.Y == move.FromY);
            var target = Pieces.FirstOrDefault(it => it.Piece.X == move.ToX && it.Piece.Y == move.ToY);
            if (source == null) throw new Exception("Didn't find the piece to move");
            Game.Move(index);
            if (move.IsScore)
            {
                source.Piece.Remove();
            }
            else
            {
                source.Piece.Move(move.ToX, move.ToY);
            }
            if (target != null)
            {
                target.Piece.Remove();
            }
            source.Update();
            target?.Update();
            Update();
        }

        private void Update()
        {
            Updated?.Invoke();
            PromptPlayer();
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
            EndGame();
            _timer.Stop();
            _timer.Elapsed -= TimerElapsed;
        }
    }
}
