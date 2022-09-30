namespace Common.ObservableProperty
{
    public class BeforeCollectionChangedEventArgs<T> : AfterCollectionChangedEventArgs<T>
    {
        public bool ChangeEnabled { get; set; }


        public BeforeCollectionChangedEventArgs(CollectionOperation collectionOperation, T item) : base(collectionOperation, item)
        {
            ChangeEnabled = true;
        }
    }
}