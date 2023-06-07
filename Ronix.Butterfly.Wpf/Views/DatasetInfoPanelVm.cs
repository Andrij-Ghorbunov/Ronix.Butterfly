using Ronix.Butterfly.Wpf.Backpropagation;
using Ronix.Framework.Mvvm;

namespace Ronix.Butterfly.Wpf.Views
{
    public class DatasetInfoPanelVm: ViewModelBase
    {
        private int _winningCount;

        public int WinningCount
        {
            get => _winningCount;
            set => SetValue(ref _winningCount, value);
        }

        private int _losingCount;

        public int LosingCount
        {
            get => _losingCount;
            set => SetValue(ref _losingCount, value);
        }

        private int _drawCount;

        public int DrawCount
        {
            get => _drawCount;
            set => SetValue(ref _drawCount, value);
        }

        private int _totalCount;

        public int TotalCount
        {
            get => _totalCount;
            set => SetValue(ref _totalCount, value);
        }

        public void Load(DatasetFile dataset)
        {
            WinningCount = dataset.Winning.Count;
            LosingCount = dataset.Losing.Count;
            DrawCount = dataset.Draws.Count;
            TotalCount = WinningCount + LosingCount + DrawCount;
        }
    }
}
