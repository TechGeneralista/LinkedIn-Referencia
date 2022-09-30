using System;


namespace Common.NotifyProperty
{
    public interface IPropertyArray<T>
    {
        event Action<T[],T[]> OnValueChanged;

        T[] PreviousValue { get; }
        T[] Value { get; }


        void DisableNextValueChangeEvents();
        void ForEach(Action<T> action);
        bool Contains(T item);
        void Add(T item);
        void AddRange(T[] items);
        void ReAddRange(T[] items);
        void Remove(T item);
        void Clear();
    }
}