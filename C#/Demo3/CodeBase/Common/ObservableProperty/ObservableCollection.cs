namespace Common.ObservableProperty
{
    public enum CollectionOperation { Add, Clear, Remove }
    public delegate void BeforeCollectionChangedEventHandler<T>(object sender, BeforeCollectionChangedEventArgs<T> e);
    public delegate void AfterCollectionChangedEventHandler<T>(object sender, AfterCollectionChangedEventArgs<T> e);

    public class ObservableCollection<T> : ObservableValueBase, IObservableCollection<T>, IReadOnlyObservableCollection<T>
    {
        public event BeforeCollectionChangedEventHandler<T> BeforeCollectionChanged;
        public event AfterCollectionChangedEventHandler<T> AfterCollectionChanged;

        public T[] Collection
        {
            get => collection;

            private set
            {
                BeforeCollectionChangedEventArgs<T> beforeCollectionChangedEventArgs = new BeforeCollectionChangedEventArgs<T>(collectionOperation, item);
                BeforeCollectionChanged?.Invoke(this, beforeCollectionChangedEventArgs);

                if(beforeCollectionChangedEventArgs.ChangeEnabled)
                {
                    AfterCollectionChangedEventArgs<T> afterCollectionChangedEventArgs = new AfterCollectionChangedEventArgs<T>(collectionOperation, item);
                    collection = value;
                    AfterCollectionChanged?.Invoke(this, afterCollectionChangedEventArgs);
                    OnPropertyChanged();
                }
            }
        }


        T[] collection;
        CollectionOperation collectionOperation;
        T item;


        public ObservableCollection()
            => collection = new T[0];

        public IObservableCollection<T> ToSettable() => this;

        public void Add(T item)
        {
            this.item = item;
            collectionOperation = CollectionOperation.Add;
            Collection = Collection.Add(item);
        }

        public void Clear()
        {
            item = default(T);
            collectionOperation = CollectionOperation.Clear;
            Collection = Collection.Clear();
        }

        public void Remove(T item)
        {
            this.item = item;
            collectionOperation = CollectionOperation.Remove;
            Collection = Collection.Remove(item);
        }
    }
}
