using System;


namespace Common.NotifyProperty
{
    public interface ISettableObservableProperty<T>
    {
        event Action<T,T> ValueChanged;

        T OldValue { get; }
        T Value {get; set;}
    }
}