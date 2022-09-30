namespace Common.ObservableProperty
{
    public interface IObservableCollection<T>
    {
        event BeforeCollectionChangedEventHandler<T> BeforeCollectionChanged;
        event AfterCollectionChangedEventHandler<T> AfterCollectionChanged;

        T[] Collection { get; }

        void Add(T item);
        void Clear();
        void Remove(T item);
    }
}