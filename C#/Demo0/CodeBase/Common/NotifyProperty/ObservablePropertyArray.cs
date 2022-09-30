using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Common.NotifyProperty
{
    public class ObservablePropertyArray<T> : INotifyPropertyChanged, ISettableObservablePropertyArray<T>, INonSettableObservablePropertyArray<T>
    {
        public event Action<T[],T[]> CurrentValueChanged;
        public event PropertyChangedEventHandler PropertyChanged;


        public T[] PreviousValue { get; private set; }

        public T[] CurrentValue
        {
            get => value;

            private set
            {
                PreviousValue = this.value;
                this.value = value;
                OnPropertyChanged();
                CurrentValueChanged?.Invoke(PreviousValue, CurrentValue);
            }
        }


        T[] value;


        public ObservablePropertyArray() => value = new T[0];
        public ObservablePropertyArray(T[] initialValue) => value = initialValue;


        public bool Contains(T item)
        {
            return Array.Exists(CurrentValue, i => i.Equals(item));
        }

        public void Add(T item)
        {
            CurrentValue = CurrentValue.Add(item);
        }

        public void AddRange(T[] items)
        {
            foreach (T item in items)
                CurrentValue = CurrentValue.Add(item);
        }

        public void Remove(T item)
        {
            CurrentValue = CurrentValue.Remove(item);
        }

        public void Clear()
        {
            CurrentValue = CurrentValue.Clear();
        }

        public void ForceAdd(T item) => Add(item);
        public void ForceAddRange(T[] items) => AddRange(items);
        public void ForceRemove(T item) => Remove(item);
        public void ForceClear() => Clear();

        void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
