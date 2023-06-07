using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ronix.Butterfly.Wpf.Views
{
    public class GamePieceVisualLite
    {
        public FrameworkElement Visual { get; }

        public Canvas Canvas { get; }

        public GameBoardLite Parent { get; }

        public bool IsAnimationPlayed { get; set; }
        public bool IsRemoved { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        private int _x, _y;

        private double _animation;

        public GamePieceVisualLite(int x, int y, bool side, FrameworkElement visual, Canvas canvas, GameBoardLite parent)
        {
            Visual = visual;
            Canvas = canvas;
            Parent = parent;

            _x = x;
            _y = y;
            X = x;
            Y = y;
        }

        public void Remove()
        {
            Parent.RemovePiece(this);
            Canvas.Children.Remove(Visual);
        }

        public void Tick()
        {
            if (!IsAnimationPlayed) return;
            _animation -= 0.04;
            Canvas.SetTop(Visual, GameBoardView.CellHeight * ((1 - _animation) * X + _animation * _x));
            Canvas.SetLeft(Visual, GameBoardView.CellWidth * ((1 - _animation) * Y + _animation * _y));
            if (IsRemoved)
                Visual.Opacity = _animation;
            if (_animation <= 0)
            {
                _animation = 1;
            }
        }

        public void SetExactPosition()
        {
            Canvas.SetTop(Visual, GameBoardView.CellHeight * X);
            Canvas.SetLeft(Visual, GameBoardView.CellWidth * Y);
        }

        public void Update()
        {
            if (_x == X && _y == Y && !IsRemoved) return;
            IsAnimationPlayed = true;
            _animation = 1;
        }
    }

}
