using Ronix.Butterfly.Wpf.Backpropagation;
using Ronix.Butterfly.Wpf.Evolution;
using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ronix.Butterfly.Wpf.Views
{
    public class TrainingSetupVm: ViewModelBase
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value, OnNameChanged);
        }

        private void OnNameChanged(string newValue)
        {
            Title = $"Enable training for {newValue}";
        }

        private string _title;

        public string Title
        {
            get => _title;
            set => SetValue(ref _title, value);
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        private double _learningRate = 0.1;

        public double LearningRate
        {
            get => _learningRate;
            set => SetValue(ref _learningRate, value);
        }

        private bool _hasMaxBlock = false;

        public bool HasMaxBlock
        {
            get => _hasMaxBlock;
            set => SetValue(ref _hasMaxBlock, value);
        }

        private int _maxBlock = 100;

        public int MaxBlock
        {
            get => _maxBlock;
            set => SetValue(ref _maxBlock, value);
        }

        private int _numberOfIterations = 10;

        public int NumberOfIterations
        {
            get => _numberOfIterations;
            set => SetValue(ref _numberOfIterations, value);
        }

        public ICommand PrepareForEvolution { get; }

        public TrainingVm Parent { get; }

        public Func<DatasetFile, List<DatasetItem>> ItemsSelector { get; }

        public TrainingSetupVm(TrainingVm parent, Func<DatasetFile, List<DatasetItem>> itemsSelector)
        {
            Parent = parent;
            ItemsSelector = itemsSelector;
            PrepareForEvolution = new Command(PrepareForEvolutionCommand);
        }

        private void PrepareForEvolutionCommand()
        {
            if (Parent.Dataset == null)
            {
                MessageBox.Show("No dataset selected!");
                return;
            }
            var items = ItemsSelector(Parent.Dataset);
            Generators.Dataset1 = Parent.PrepareDataset1(items);
            Generators.Dataset50 = Parent.PrepareDataset50(items);
            Generators.LearningRate = LearningRate;
            Generators.Iterations = NumberOfIterations;
            Generators.MaxBlockSize = GetMaxBlock();
            Parent.Evolution.IsPreTrainPrepared = true;
            MessageBox.Show($"Evolution Pre-Train data enabled ({Generators.Dataset1.GetItemsCount()} items for '38-1' architecture or {Generators.Dataset50.GetItemsCount()} items for '38-50')");
        }

        private int? GetMaxBlock()
        {
            if (HasMaxBlock) return MaxBlock;
            return null;
        }
    }
}
