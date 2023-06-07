using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Ronix.Framework.WpfToolkit.Collections
{
    public class UpdatableCollectionViewSource : CollectionViewSource
    {
        private readonly List<INotifyPropertyChanged> _duplicateSource = new List<INotifyPropertyChanged>();

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Data.CollectionViewSource.Source"/> property changes.
        /// </summary>
        /// <param name="oldSource">The old value of the <see cref="P:System.Windows.Data.CollectionViewSource.Source"/> property.</param>
        /// <param name="newSource">The new value of the <see cref="P:System.Windows.Data.CollectionViewSource.Source"/> property.</param>
        protected override void OnSourceChanged(object oldSource, object newSource)
        {
            var oldValue = oldSource as IEnumerable;
            if (oldValue != null)
                UnsubscribeSourceEvents(oldValue);

            var newValue = newSource as IEnumerable;
            if (newValue != null)
                SubscribeSourceEvents(newValue);

            base.OnSourceChanged(oldSource, newSource);
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bool refresh = SortDescriptions.Any(sort => sort.PropertyName == e.PropertyName)
                           || GroupDescriptions.OfType<PropertyGroupDescription>().Any(
                               propertyGroup => propertyGroup.PropertyName == e.PropertyName);

            if (refresh)
                View.Refresh();
        }

        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in _duplicateSource)
                    item.PropertyChanged -= ItemPropertyChanged;
                _duplicateSource.Clear();
                var source = sender as IEnumerable;
                if (source != null)
                    foreach (var item in source.OfType<INotifyPropertyChanged>())
                        SubscribeItemEvents(item);
                return;
            }
            if (e.OldItems != null)
                foreach (var item in e.OldItems.OfType<INotifyPropertyChanged>())
                    UnsubscribeItemEvents(item);
            if (e.NewItems != null)
                foreach (var item in e.NewItems.OfType<INotifyPropertyChanged>())
                    SubscribeItemEvents(item);
        }

        private void SubscribeItemEvents(INotifyPropertyChanged item)
        {
            item.PropertyChanged += ItemPropertyChanged;
            _duplicateSource.Add(item);
        }

        private void UnsubscribeItemEvents(INotifyPropertyChanged item)
        {
            item.PropertyChanged -= ItemPropertyChanged;
            _duplicateSource.Remove(item);
        }
        
        private void SubscribeSourceEvents(IEnumerable source)
        {
            var notify = source as INotifyCollectionChanged;

            if (notify != null)
                notify.CollectionChanged += SourceCollectionChanged;

            foreach (var item in source.OfType<INotifyPropertyChanged>())
                SubscribeItemEvents(item);
        }

        private void UnsubscribeSourceEvents(IEnumerable source)
        {
            var notify = source as INotifyCollectionChanged;

            if (notify != null)
                notify.CollectionChanged -= SourceCollectionChanged;

            foreach (var item in source.OfType<INotifyPropertyChanged>())
                UnsubscribeItemEvents(item);
        }
    }
}
