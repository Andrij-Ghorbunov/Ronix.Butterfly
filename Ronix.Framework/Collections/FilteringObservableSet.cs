using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

using Ronix.Framework.Mvvm;

namespace Ronix.Framework.Collections
{
    /// <summary>
    /// Extends <see cref="FilteredObservableSet{T}"/> functionality by observing property updates of each element.
    /// </summary>
    /// <typeparam name="T">Type of items in the collection. Has to implement <see cref="INotifyPropertyChanged"/> interface.</typeparam>
    public class FilteringObservableSet<T> : FilteredObservableSet<T> where T : INotifyPropertyChanged
    {
        private List<T> _duplicateSource;

        /// <summary>
        /// Mother always told me not to call virtual members in constructor...
        /// </summary>
        private void TryInit()
        {
            if (_duplicateSource == null)
                _duplicateSource = new List<T>();
        }

        private readonly string _propertyName;

        /// <summary>
        /// Initializes a new instance of <see cref="FilteringObservableSet{T}"/>, based on a certain collection.
        /// </summary>
        /// <param name="source">The source collection for elements.</param>
        /// <param name="filter">Condition for filtering elements.</param>
        /// <param name="property">Property of the filter predicate. If specified, updates to other properties are ignored.</param>
        public FilteringObservableSet(ObservableCollection<T> source, Func<T, bool> filter = null, Expression<Func<T>> property = null)
            : base(source, filter)
        {
            TryInit();
            if (property != null)
                _propertyName = ViewModelBase.GetMemberName(property);
            foreach (var item in source)
            {
                item.PropertyChanged += ItemPropertyChanged;
                _duplicateSource.Add(item);
            }
        }

        protected override void ProcessSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var item in e.OldItems.OfType<T>())
                {
                    item.PropertyChanged -= ItemPropertyChanged;
                    _duplicateSource.Add(item);
                }
            if (e.NewItems != null)
                foreach (var item in e.NewItems.OfType<T>())
                {
                    item.PropertyChanged += ItemPropertyChanged;
                    _duplicateSource.Remove(item);
                }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_propertyName != null && e.PropertyName != _propertyName)
                return;
            var item = (T)sender;
            var contains = Contains(item);
            var filter = Filter(item);
            if (contains && !filter)
                Remove(item);
            else if (!contains && filter)
                Add(item);
        }

        /// <summary>
        /// Reapplies the filter to the collection.
        /// </summary>
        public override void Refilter()
        {
            TryInit();

            foreach (var item in _duplicateSource)
            {
                item.PropertyChanged -= ItemPropertyChanged;
            }
            _duplicateSource.Clear();

            base.Refilter();

            foreach (var item in Source)
            {
                item.PropertyChanged += ItemPropertyChanged;
                _duplicateSource.Add(item);
            }
        }
    }
}
