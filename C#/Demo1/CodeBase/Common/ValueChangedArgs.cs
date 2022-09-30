using System;


namespace Common
{
    public class ValueChangedArgs<T> : EventArgs
    {
        public T NewValue { get; }
        public T OldValue { get; }


        public ValueChangedArgs(T newValue, T oldValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}
