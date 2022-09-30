using System;


namespace Common.NotifyProperty
{
    public interface ISettableObservableProperty<T>
    {
        event Action<T,T> CurrentValueChanged;

        T PreviousValue { get; }
        T CurrentValue { get; set;}
    }
}