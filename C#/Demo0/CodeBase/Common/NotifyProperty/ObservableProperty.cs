using System;


namespace Common.NotifyProperty
{
    public class ObservableProperty<T> : NotifyPropertyBase, ISettableObservableProperty<T>, INonSettableObservableProperty<T>
    {
        public event Action<T,T> CurrentValueChanged;


        public bool NextCurrentValueChangedDisabled { get; set; }
        public T PreviousValue { get; private set; }
        public T CurrentValue
        {
            get => currentValue;

            set
            {
                PreviousValue = currentValue;
                currentValue = value;
                OnPropertyChanged();

                if (NextCurrentValueChangedDisabled)
                    NextCurrentValueChangedDisabled = false;
                else
                    CurrentValueChanged?.Invoke(PreviousValue, CurrentValue);
            }
        }


        T currentValue;


        public ObservableProperty() : this(default, null) { }
        public ObservableProperty(T initialValue) : this(initialValue, null) { }
        public ObservableProperty(Action<T, T> currentValueChanged) : this(default, currentValueChanged) { }

        public ObservableProperty(T initialValue, Action<T, T> currentValueChanged)
        {
            if (currentValueChanged.IsNotNull())
                CurrentValueChanged += currentValueChanged;

            CurrentValue = initialValue;
        }

        public void ForceSet(T newValue) => CurrentValue = newValue;
    }
}
