using System;


namespace Common.Prop
{
    public interface INonSettableObservableProperty<T>
    {
        event Action<T> ValueChanged;
        T Value { get; }
        void ForceSet(T newValue);
    }
}