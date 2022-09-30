namespace Common.ObservableProperty
{
    public class BeforeValueChangedEventArgs<T> : AfterValueChangedEventArgs<T>
    {
        public bool ChangeEnabled { get; set; }


        public BeforeValueChangedEventArgs(T oldValue, T newValue) : base(oldValue, newValue)
        {
            ChangeEnabled = true;
        }
    }
}
