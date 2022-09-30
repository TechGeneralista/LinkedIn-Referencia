using System;


namespace Common.Prop
{
    public interface ISettableObservableProperty<T>
    {
        event Action<T> ValueChanged;
        T Value {get; set;}
    }
}