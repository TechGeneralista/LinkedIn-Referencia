using System;


namespace Common.ObservableProperty
{
    public interface IObservableValue<T>
    {
        event BeforeValueChangedEventHandler<T> BeforeValueChanged;
        event AfterValueChangedEventHandler<T> AfterValueChanged;

        T Value { get; set; }
    }
}
