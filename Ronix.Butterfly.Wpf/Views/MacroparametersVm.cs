using Ronix.Butterfly.Wpf.Evolution;
using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Views
{
    public class MacroparametersVm : ViewModelBase
    {
        private int _participantCount;

        public int ParticipantCount
        {
            get => _participantCount;
            set => SetValue(ref _participantCount, value);
        }

        private int _topCut;

        public int TopCut
        {
            get => _topCut;
            set => SetValue(ref _topCut, value, ValidateTopCut);
        }

        private void ValidateTopCut(int oldValue, int newValue)
        {
            if (newValue < 0)
                TopCut = oldValue;
        }

        private int _freshBlood;

        public int FreshBlood
        {
            get => _freshBlood;
            set => SetValue(ref _freshBlood, value, ValidateFreshBlood);
        }

        private void ValidateFreshBlood(int oldValue, int newValue)
        {
            if (newValue < 0)
                FreshBlood = oldValue;
        }

        private int _numberOfMutations;

        public int NumberOfMutations
        {
            get => _numberOfMutations;
            set => SetValue(ref _numberOfMutations, value, ValidateNumberOfMutations);
        }

        private void ValidateNumberOfMutations(int oldValue, int newValue)
        {
            if (newValue < 0)
                NumberOfMutations = oldValue;
        }

        private double _amplitudeOfMutations;

        public double AmplitudeOfMutations
        {
            get => _amplitudeOfMutations;
            set => SetValue(ref _amplitudeOfMutations, value, ValidateAmplitudeOfMutations);
        }

        private void ValidateAmplitudeOfMutations(double oldValue, double newValue)
        {
            if (newValue < 0)
                AmplitudeOfMutations = oldValue;
        }

        public void Load(EvolutionInstance evo)
        {
            ParticipantCount = evo.ParticipantCount;
            TopCut = evo.TopCut;
            FreshBlood = evo.FreshBlood;
            NumberOfMutations = evo.NumberOfMutations;
            AmplitudeOfMutations = evo.AmplitudeOfMutations;
        }

        public void Save(EvolutionInstance evo)
        {
            evo.ParticipantCount = ParticipantCount;
            evo.TopCut = TopCut;
            evo.FreshBlood = FreshBlood;
            evo.NumberOfMutations = NumberOfMutations;
            evo.AmplitudeOfMutations = AmplitudeOfMutations;
        }
    }
}
