using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Ronix.Framework.Extensions
{
    public static class ObservableExtensions
    {
        public static ObservableCollection<T> ObservableCopy<T>(this ObservableCollection<T> @this)
        {
            var r = new ObservableCollection<T>(@this);
            @this.CollectionChanged += (sender, args) => UpdateCollection<T, T>(r, args, it => it);
            return r;
        }

        public static ObservableCollection<TResult> ObservableSelect<TSource, TResult>(this ObservableCollection<TSource> @this, Func<TSource, TResult> selector)
        {
            var r = new ObservableCollection<TResult>(@this.Select(selector));
            @this.CollectionChanged += (sender, args) => UpdateCollection(r, args, selector);
            return r;
        }

        private static void UpdateCollection<TSource, TResult>(ObservableCollection<TResult> target, NotifyCollectionChangedEventArgs e, Func<TSource, TResult> selector)
        {
            int index;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    target.Clear();
                    return;
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems == null)
                        return;
                    index = e.NewStartingIndex;
                    foreach (var source in e.NewItems.OfType<TSource>().Select(selector).ToArray())
                    {
                        target.CarefulInsert(source, index++);
                    }
                    return;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems == null)
                        return;
                    foreach (var source in e.OldItems.OfType<TSource>().Select(selector).ToArray())
                    {
                        target.Remove(source);
                    }
                    return;
                case NotifyCollectionChangedAction.Replace:
                    index = e.NewStartingIndex;
                    var newIt = e.NewItems.OfType<TSource>().Select(selector).ToArray();
                    var total = newIt.Length;
                    for (var i = 0; i < total; i++)
                    {
                        target[index++] = newIt[i];
                    }
                    return;
#if !SILVERLIGHT
                case NotifyCollectionChangedAction.Move:
                    index = e.NewStartingIndex - 1;
                    foreach (var item in e.OldItems.OfType<TSource>().Select(selector).ToArray())
                        target.Move(target.IndexOf(item), index++);
                    return;
#endif
            }
        }

        public static void CarefulInsert<T>(this IList<T> @this, T item, int index)
        {
            if (index < 0)
            {
                @this.Insert(0, item);
                return;
            }
            if (index >= @this.Count)
            {
                @this.Add(item);
                return;
            }
            @this.Insert(index, item);
        }
    }
}
