using System;


namespace Common.NotifyProperty
{
    public interface ISettableObservableProperty<T>
    {
        event Action<T,T> CurrentValueChanged;

        bool NextCurrentValueChangedDisabled { get; set; }
        T PreviousValue { get; }
        T CurrentValue { get; set;}
    }
}