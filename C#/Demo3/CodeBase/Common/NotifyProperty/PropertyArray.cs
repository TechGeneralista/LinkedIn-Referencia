using System;
using System.ComponentModel;
using System.Linq;


namespace Common.NotifyProperty
{
    public class PropertyArray<T> : PropertyBase, INotifyPropertyChanged, IPropertyArray<T>, IReadOnlyPropertyArray<T>
    {
        public event Action<T[], T[]> OnValueChanging;
        public event Action<T[], T[]> OnValueChanged;


        public T[] PreviousValue { get; private set; }

        public T[] Value
        {
            get => value;

            private set
            {
                if (!nextOnValueChangeEventsDisabled)
                    OnValueChanging?.Invoke(PreviousValue, Value);

                PreviousValue = this.value;
                this.value = value;
                OnPropertyChanged();

                if (!nextOnValueChangeEventsDisabled)
                    OnValueChanged?.Invoke(PreviousValue, Value);

                if (nextOnValueChangeEventsDisabled)
                    nextOnValueChangeEventsDisabled = false;
            }
        }


        T[] value;
        bool nextOnValueChangeEventsDisabled;


        public PropertyArray() => value = new T[0];
        public PropertyArray(T[] initialValue) => value = initialValue;

        public void DisableNextValueChangeEvents() => nextOnValueChangeEventsDisabled = true;

        public bool Contains(T item) => Array.Exists(Value, i => i.Equals(item));

        public void Add(T item) => Value = Value.Add(item);

        public void AddRange(T[] items)
        {
            T[] newValue = new T[0];
            newValue = newValue.AddRange(Value);
            newValue = newValue.AddRange(items);
            Value = newValue.ToArray();
        }

        public void ReAddRange(T[] items)
        {
            nextOnValueChangeEventsDisabled = true;
            Clear();
            AddRange(items);
        }

        public void Remove(T item) => Value = Value.Remove(item);

        public void Clear() => Value = Value.Clear();

        public void ForEach(Action<T> action)
        {
            if (action.IsNull())
                throw new ArgumentNullException(nameof(action));

            foreach (T item in value)
                action.Invoke(item);
        }
    }
}
