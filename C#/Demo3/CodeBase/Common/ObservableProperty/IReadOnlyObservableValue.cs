using System;


namespace Common.ObservableProperty
{
    public interface IReadOnlyObservableValue<T>
    {
        event BeforeValueChangedEventHandler<T> BeforeValueChanged;
        event AfterValueChangedEventHandler<T> AfterValueChanged;

        T Value { get; }

        IObservableValue<T> ToSettable();
    }
}
