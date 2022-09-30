using System;


namespace Common.NotifyProperty
{
    public interface INonSettableObservableProperty<T>
    {
        event Action<T,T> ValueChanged;

        T OldValue { get; }
        T Value { get; }

        void ForceSet(T value);
    }
}