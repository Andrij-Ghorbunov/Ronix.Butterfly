using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Ronix.Framework.Collections
{
    /// <summary>
    /// An <see cref="ObservableCollection{T}"/> filtered by a predicate. Keeps item order of the original collection.
    /// </summary>
    /// <typeparam name="T">Type of items in the collection.</typeparam>
    public class FilteredObservableCollection<T> : ObservableCollection<T>
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
        /// Initializes a new instance of <see cref="FilteredObservableCollection{T}"/>, based on a certain collection.
        /// </summary>
        /// <param name="source">The source collection for elements.</param>
        /// <param name="filter">Condition for filtering elements.</param>
        public FilteredObservableCollection(ObservableCollection<T> source, Func<T, bool> filter = null)
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
            ProcessSourceCollectionChanged(e);
            int index;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    Refilter();
                    return;

                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex == Source.Count - 1)
                    { // Source.Add
                        foreach (var item in e.NewItems.OfType<T>().Where(item => _filter(item)))
                            Add(item);
                    }
                    else
                    { // Source.Insert
                        index = GetIndex(e.NewStartingIndex);
                        foreach (var item in e.NewItems.OfType<T>().Where(item => _filter(item)))
                            Insert(index++, item);
                    }
                    return;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<T>().Where(item => _filter(item)))
                        Remove(item);
                    return;

                case NotifyCollectionChangedAction.Replace:
                    index = GetIndex(e.NewStartingIndex);

                    var oldIt = e.OldItems.OfType<T>().ToArray();
                    var newIt = e.NewItems.OfType<T>().ToArray();
                    int total = oldIt.Length;
                    if (total != newIt.Length)
                        throw new Exception("Wrong notification");
                    for (int i = 0; i < total; i++)
                    {
                        bool oldMatch = _filter(oldIt[i]);
                        bool newMatch = _filter(newIt[i]);
                        if (oldMatch && newMatch)
                            this[index++] = newIt[i];
                        else if (oldMatch)
                            RemoveAt(index);
                        else if (newMatch)
                            Insert(index++, newIt[i]);
                    }

                    // simple implementation (does not produce Replace notification, even if possible):

                    //foreach (var item in e.OldItems.OfType<T>().Where(item => _filter(item)))
                    //    Remove(item);
                    //foreach (var item in e.NewItems.OfType<T>().Where(item => _filter(item)))
                    //    Insert(index++, item);

                    return;

#if !SILVERLIGHT
                case NotifyCollectionChangedAction.Move:
                    index = GetIndex(e.NewStartingIndex) - 1;
                    foreach (var item in e.OldItems.OfType<T>().Where(item => _filter(item)))
                        Move(IndexOf(item), index++);
                    return;
#endif
            }
        }

        private int GetIndex(int sourceIndex)
        {
            do
            {
                sourceIndex--;
                if (sourceIndex < 0) return 0;
            } while (!_filter(Source[sourceIndex]));
            return IndexOf(Source[sourceIndex]) + 1;
        }
    }
}
