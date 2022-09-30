using System;


namespace Common.NotifyProperty
{
    public interface IReadOnlyPropertyArray<T>
    {
        event Action<T[], T[]> OnValueChanging;
        event Action<T[],T[]> OnValueChanged;

        T[] PreviousValue { get; }
        T[] Value { get; }


        void ForEach(Action<T> action);
        bool Contains(T item);
    }
}