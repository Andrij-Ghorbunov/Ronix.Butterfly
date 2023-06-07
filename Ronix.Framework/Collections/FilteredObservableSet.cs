using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Ronix.Framework.Collections
{
    /// <summary>
    /// An <see cref="ObservableCollection{T}"/> filtered by a predicate. Doesn't keep item order.
    /// </summary>
    /// <typeparam name="T">Type of items in the collection.</typeparam>
    public class FilteredObservableSet<T> : ObservableCollection<T>
    {
        private static readonly Func<T, bool> NullFilter = arg => true;

        /// <summary>
        /// Gets source collection that contains all elements (including those not satisfying the filter condition).
        /// </summary>
        public ObservableCollection<T> Source { get; private set; }

        private Func<T, bool> _filter;

        /// <summary>
        /// Gets or sets the condition to filter elements.
        /// </summary>
        public Func<T, bool> Filter
        {
            get { return _filter; }
            set
            {
                if (_filter == value) return;
                if (value == null)
                {
                    Filter = NullFilter;
                    return;
                }
                _filter = value;
                Refilter();
            }
        }

        /// <summary>
        /// Reapplies the filter to the collection.
        /// </summary>
        public virtual void Refilter()
        {
            Clear();
            foreach (var item in Source.Where(item => _filter(item)))
                Add(item);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FilteredObservableSet{T}"/>, based on a certain collection.
        /// </summary>
        /// <param name="source">The source collection for elements.</param>
        /// <param name="filter">Condition for filtering elements.</param>
        public FilteredObservableSet(ObservableCollection<T> source, Func<T, bool> filter = null)
        {
            Source = source;

            Source.CollectionChanged += SourceCollectionChanged;

            Filter = filter ?? NullFilter;
        }

        /// <summary>
        /// When overridden in a derived class, triggers before the main routine of updating elements.
        /// </summary>
        /// <param name="e">Notification about changes in the source collection.</param>
        protected virtual void ProcessSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
        }

        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
#if !SILVERLIGHT
            if (e.Action == NotifyCollectionChangedAction.Move)
                return;
#endif
            ProcessSourceCollectionChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Refilter();
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var oldItems = e.OldItems.OfType<T>().ToList();
                var newItems = e.NewItems.OfType<T>().ToList();
                var count = oldItems.Count;
                if (count != newItems.Count)
                    throw new Exception("Wrong notification");
                for (int i = 0; i < count; i++)
                {
                    var oldFits = Filter(oldItems[i]);
                    var newFits = Filter(newItems[i]);
                    if (oldFits && newFits)
                        this[IndexOf(oldItems[i])] = newItems[i];
                    else if (oldFits)
                        Remove(oldItems[i]);
                    else if (newFits)
                        Add(newItems[i]);
                }
                return;
            }
            if (e.OldItems != null)
                foreach (var item in e.OldItems.OfType<T>().Where(item => _filter(item)))
                    Remove(item);
            if (e.NewItems != null)
                foreach (var item in e.NewItems.OfType<T>().Where(item => _filter(item)))
                    Add(item);
        }
    }
}
