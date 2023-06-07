namespace Ronix.Butterfly
{
    public class GamePiece
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Side { get; set; }

        public bool IsRemoved { get; private set; }

        public void Remove()
        {
            IsRemoved = true;
        }

        public void Move(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
