using System;


namespace Common.NotifyProperty
{
    public interface INonSettableObservableProperty<T>
    {
        event Action<T,T> CurrentValueChanged;

        T PreviousValue { get; }
        T CurrentValue { get; }

        void ForceSet(T newValue);
    }
}