using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;


namespace Common.NonGenerics
{
    public class ThreadSafeObservableCollection : ObservableCollection<object>
    {
        public object Lock { get; } = new object();

        public ThreadSafeObservableCollection()
            => BindingOperations.EnableCollectionSynchronization(this, Lock);

        public ThreadSafeObservableCollection(IEnumerable items) : this()
        {
            lock(Lock)
                AddRange(items);
        }

        public void AddRange(IEnumerable items)
        {
            foreach (object item in items)
                Add(item);
        }

        public void SortInPlace(SortDelegate sortDelegate)
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
