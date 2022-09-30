using System;


namespace Common.NotifyProperty
{
    public interface IReadOnlyProperty<T>
    {
        event Action<T, T> OnValueChanging;
        event Action<T, T> OnValueChanged;

        T PreviousValue { get; }
        T Value { get; }
    }
}