using System;


namespace Common.NotifyProperty
{
    public class Property<T> : PropertyBase, IProperty<T>, IReadOnlyProperty<T>
    {
        public event Action<T, T> OnValueChanging;
        public event Action<T, T> OnValueChanged;

        public T PreviousValue { get; private set; }
        public T Value
        {
            get => value;

            set
            {
                if (!nextOnValueChangeEventsDisabled)
                    OnValueChanging?.Invoke(this.value, value);

                PreviousValue = this.value;
                this.value = value;
                OnPropertyChanged();

                if (!nextOnValueChangeEventsDisabled)
                    OnValueChanged?.Invoke(PreviousValue, this.value);

                if (nextOnValueChangeEventsDisabled)
                    nextOnValueChangeEventsDisabled = false;
            }
        }


        T value;
        bool nextOnValueChangeEventsDisabled;

        public Property() : this(default, null) { }
        public Property(T initialValue) : this(initialValue, null) { }
        public Property(Action<T, T> currentValueChanged) : this(default, currentValueChanged) { }

        public Property(T initialValue, Action<T, T> onValueChanged)
        {
            if (onValueChanged.IsNotNull())
                OnValueChanged += onValueChanged;

            Value = initialValue;
        }

        public void DisableNextOnValueChangeEvents() => nextOnValueChangeEventsDisabled = true;
    }
}
