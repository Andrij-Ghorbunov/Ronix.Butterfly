using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Views
{
    public class ViewModelWithStopwatch: ViewModelBase
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Queue<TimeSpan> _lastRounds = new Queue<TimeSpan>();

        private TimeSpan _lastRoundTime;

        public TimeSpan LastRoundTime
        {
            get => _lastRoundTime;
            set => SetValue(ref _lastRoundTime, value);
        }

        private TimeSpan _averageRoundTime;

        public TimeSpan AverageRoundTime
        {
            get => _averageRoundTime;
            set => SetValue(ref _averageRoundTime, value);
        }

        protected void StartRound()
        {
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        protected void FinishRound()
        {
            _stopwatch.Stop();
            var time = _stopwatch.Elapsed;
            LastRoundTime = time;
            _lastRounds.Enqueue(time);
            if (_lastRounds.Count > 10)
                _lastRounds.Dequeue();
            AverageRoundTime = TimeSpan.FromMilliseconds(_lastRounds.Average(it => it.TotalMilliseconds));
        }
    }
}
