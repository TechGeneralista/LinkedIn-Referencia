using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Common.NotifyProperty
{
    public class ObservableProperty<T> : INotifyPropertyChanged, ISettableObservableProperty<T>, INonSettableObservableProperty<T>
    {
        public event Action<T,T> CurrentValueChanged;
        public event PropertyChangedEventHandler PropertyChanged;


        public T PreviousValue { get; private set; }
        public T CurrentValue
        {
            get => value;

            set
            {
                PreviousValue = this.value;
                this.value = value;
                OnPropertyChanged();
                CurrentValueChanged?.Invoke(PreviousValue, CurrentValue);
            }
        }


        T value;


        public ObservableProperty() => value = default;
        public ObservableProperty(T initialValue) => value = initialValue;


        public void ForceSet(T newValue) => CurrentValue = newValue;
        void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
