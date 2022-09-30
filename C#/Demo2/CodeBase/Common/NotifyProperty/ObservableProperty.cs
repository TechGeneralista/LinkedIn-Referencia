using System;


namespace Common.NotifyProperty
{
    public class ObservableProperty<T> : NotifyBase, ISettableObservableProperty<T>, INonSettableObservableProperty<T>
    {
        public event Action<T,T> ValueChanged;

        public T OldValue { get; private set; }

        public T Value
        {
            get => v;

            set
            {
                OldValue = v;
                v = value;
                OnPropertyChanged();
                ValueChanged?.Invoke(OldValue, v);
            }
        }

        T v;

        public ObservableProperty() => v = default;
        public ObservableProperty(T initialValue) => v = initialValue;

        public void ForceSet(T newValue) => Value = newValue;
    }
}
