using System;


namespace SmartVisionClientApp.Common
{
    public interface ISettableObservableProperty<T>
    {
        event Action<T> ValueChanged;
        T Value {get; set;}
    }
}