using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Views
{
    public class NoiseVm: ViewModelBase
    {
        private Random Random = new Random();

        private bool _isInputNoiseEnabled;

        public bool IsInputNoiseEnabled
        {
            get => _isInputNoiseEnabled;
            set => SetValue(ref _isInputNoiseEnabled, value);
        }

        private double _inputNoiseAmplitude = 0.1;

        public double InputNoiseAmplitude
        {
            get => _inputNoiseAmplitude;
            set => SetValue(ref _inputNoiseAmplitude, value);
        }

        private bool _isOutputNoiseEnabled;

        public bool IsOutputNoiseEnabled
        {
            get => _isOutputNoiseEnabled;
            set => SetValue(ref _isOutputNoiseEnabled, value);
        }

        private double _outputNoiseAmplitude = 0.1;

        public double OutputNoiseAmplitude
        {
            get => _outputNoiseAmplitude;
            set => SetValue(ref _outputNoiseAmplitude, value);
        }

        public void ProcessInput(double[] input)
        {
            if (!IsInputNoiseEnabled) return;
            ProcessArray(input, InputNoiseAmplitude);
        }

        public void ProcessOutput(double[] output)
        {
            if (!IsInputNoiseEnabled) return;
            ProcessArray(output, OutputNoiseAmplitude);
        }

        private void ProcessArray(double[] array, double magnitude)
        {
            var len = array.Length;
            for (var i = 0; i < len; i++)
            {
                var noise = (Random.NextDouble() - Random.NextDouble()) * magnitude;
                array[i] += noise;
            }
        }
    }
}
