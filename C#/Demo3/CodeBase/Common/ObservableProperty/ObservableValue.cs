using System;


namespace Common.ObservableProperty
{
    public delegate void BeforeValueChangedEventHandler<T>(object sender, BeforeValueChangedEventArgs<T> e);
    public delegate void AfterValueChangedEventHandler<T>(object sender, AfterValueChangedEventArgs<T> e);

    public class ObservableValue<T> : ObservableValueBase, IObservableValue<T>, IReadOnlyObservableValue<T>
    {
        public event BeforeValueChangedEventHandler<T> BeforeValueChanged;
        public event AfterValueChangedEventHandler<T> AfterValueChanged;

        public T Value 
        {
            get => value;

            set
            {
                T oldValue = this.value;
                BeforeValueChangedEventArgs<T> beforeChangedEventArgs = new BeforeValueChangedEventArgs<T>(oldValue, value);
                BeforeValueChanged?.Invoke(this, beforeChangedEventArgs);

                if(beforeChangedEventArgs.ChangeEnabled)
                {
                    AfterValueChangedEventArgs<T> afterChangedEventArgs = new AfterValueChangedEventArgs<T>(oldValue, value);
                    this.value = value;
                    AfterValueChanged?.Invoke(this, afterChangedEventArgs);
                    OnPropertyChanged();
                }
            }
        }


        T value;

        public ObservableValue(T initialValue = default(T))
            => value = initialValue;

        public IObservableValue<T> ToSettable() => this;
    }
}
