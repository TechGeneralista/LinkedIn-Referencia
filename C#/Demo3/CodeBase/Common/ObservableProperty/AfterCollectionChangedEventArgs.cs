namespace Common.ObservableProperty
{
    public class AfterCollectionChangedEventArgs<T>
    {
        public CollectionOperation CollectionOperation { get; }
        public T Item { get; }


        public AfterCollectionChangedEventArgs(CollectionOperation collectionOperation, T item)
        {
            CollectionOperation = collectionOperation;
            Item = item;
        }
    }
}