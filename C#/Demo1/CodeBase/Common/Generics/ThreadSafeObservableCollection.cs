using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;


namespace Common.Generics
{
    public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {
        public object Lock { get; } = new object();

        public ThreadSafeObservableCollection()
            => BindingOperations.EnableCollectionSynchronization(this, Lock);

        public ThreadSafeObservableCollection(IEnumerable<T> items) : this()
        {
            lock(Lock)
                AddRange(items);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
                Add(item);
        }

        public void SortInPlace(SortDelegate<T, T> sortDelegate)
        {
            var itemMoved = false;

            do
            {
                itemMoved = false;
                for (int currentIndex = 0; currentIndex < Count - 1; currentIndex++)
                {
                    int nextIndex = currentIndex + 1;

                    if (sortDelegate(this[currentIndex], this[nextIndex]))
                    {
                        Move(currentIndex, nextIndex);
                        itemMoved = true;
                    }
                }
            } while (itemMoved);
        }
    }
}
