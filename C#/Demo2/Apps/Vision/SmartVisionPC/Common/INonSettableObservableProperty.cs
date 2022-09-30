using System;


namespace SmartVisionClientApp.Common
{
    public interface INonSettableObservableProperty<T>
    {
        event Action<T> ValueChanged;
        T Value { get; }
    }
}