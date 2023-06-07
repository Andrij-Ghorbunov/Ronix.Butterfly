using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ronix.Butterfly.Wpf.Views
{
    public class SingleDatasetVm : ViewModelBase
    {
        public event Action<DatasetItem> OnSelectionChanged;

        public ObservableCollection<DatasetItem> Items { get; }

        private DatasetItem _selectedItem;

        public DatasetItem SelectedItem
        {
            get => _selectedItem;
            set => SetValue(ref _selectedItem, value, SelectedItemChanged);
        }

        private bool _isInverted;

        public bool IsInverted
        {
            get => _isInverted;
            set => SetValue(ref _isInverted, value, IsInvertedChanged);
        }


        private readonly Command _delete;

        public ICommand Delete => _delete;

        private void DeleteCommand()
        {
            _listReference.Remove(SelectedItem);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            Parent.Save();
        }

        private bool DeleteCommandPossible()
        {
            return SelectedItem != null;
        }

        private void SelectedItemChanged(DatasetItem newValue)
        {
            OnSelectionChanged?.Invoke(newValue);
            _delete.RaiseCanExecuteChanged();
        }

        private void IsInvertedChanged(bool newValue)
        {
            OnSelectionChanged?.Invoke(SelectedItem);
        }

        public DatasetsVm Parent { get; }

        private List<DatasetItem> _listReference;

        public SingleDatasetVm(DatasetsVm parent)
        {
            Parent = parent;
            _delete = new Command(DeleteCommand, DeleteCommandPossible);
            Items = new ObservableCollection<DatasetItem>();
        }
        
        public void Init(List<DatasetItem> items)
        {
            _listReference = items;
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
    }
}
