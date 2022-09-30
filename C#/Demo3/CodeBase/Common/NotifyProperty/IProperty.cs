using System;


namespace Common.NotifyProperty
{
    public interface IProperty<T>
    {
        event Action<T, T> OnValueChanging;
        event Action<T, T> OnValueChanged;

        T PreviousValue { get; }
        T Value { get; set;}

        void DisableNextOnValueChangeEvents();
    }
}