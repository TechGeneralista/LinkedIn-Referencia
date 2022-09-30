namespace Common.ObservableProperty
{
    public interface IReadOnlyObservableCollection<T>
    {
        event BeforeCollectionChangedEventHandler<T> BeforeCollectionChanged;
        event AfterCollectionChangedEventHandler<T> AfterCollectionChanged;

        T[] Collection { get; }

        IObservableCollection<T> ToSettable();
    }
}