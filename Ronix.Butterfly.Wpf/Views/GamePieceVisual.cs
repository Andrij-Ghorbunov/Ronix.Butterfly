using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ronix.Butterfly.Wpf.Views
{
    public class GamePieceVisual
    {
        public GamePiece Piece { get; }

        public FrameworkElement Visual { get; }

        public Canvas Canvas { get; }

        public GameBoardView Parent { get; }

        public bool IsAnimationPlayed { get; private set; }

        private int _x, _y;

        private double _animation;

        public GamePieceVisual(GamePiece piece, FrameworkElement visual, Canvas canvas, GameBoardView parent)
        {
            Piece = piece;
            Visual = visual;
            Canvas = canvas;
            Parent = parent;

            _x = piece.X;
            _y = piece.Y;

            Visual.MouseDown += VisualMouseDown;
        }

        private void VisualMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsAnimationPlayed) return;
            Parent.ClickPiece(this);
        }

        public void Remove()
        {
            Parent.RemovePiece(this);
            Canvas.Children.Remove(Visual);
            Visual.MouseDown -= VisualMouseDown;
        }

        public void Tick()
        {
            if (!IsAnimationPlayed) return;
            _animation -= 0.04;
            if (_animation <= 0)
            {
                SetExactPosition();
                IsAnimationPlayed = false;
                Parent.AnimationEnded();
                return;
            }
            Canvas.SetTop(Visual, GameBoardView.CellHeight * ((1 - _animation) * Piece.X + _animation * _x));
            Canvas.SetLeft(Visual, GameBoardView.CellWidth * ((1 - _animation) * Piece.Y + _animation * _y));
            if (Piece.IsRemoved)
                Visual.Opacity = _animation;
        }

        public void SetExactPosition()
        {
            Canvas.SetTop(Visual, GameBoardView.CellHeight * Piece.X);
            Canvas.SetLeft(Visual, GameBoardView.CellWidth * Piece.Y);
            _x = Piece.X;
            _y = Piece.Y;
            if (Piece.IsRemoved)
                Remove();
        }

        public void EndAnimation()
        {
            SetExactPosition();
            IsAnimationPlayed = false;
            Parent.AnimationEnded();
        }

        public void Update()
        {
            if (_x == Piece.X && _y == Piece.Y && !Piece.IsRemoved) return;
            IsAnimationPlayed = true;
            _animation = 1;
            Parent.AnimationStarted();
        }
    }
}
