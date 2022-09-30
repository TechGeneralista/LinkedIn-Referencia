using System;


namespace Common.NotifyProperty
{
    public interface ISettableObservablePropertyArray<T>
    {
        event Action<T[],T[]> CurrentValueChanged;

        T[] PreviousValue { get; }
        T[] CurrentValue { get; }


        bool Contains(T item);
        void Add(T item);
        void AddRange(T[] items);
        void Remove(T item);
        void Clear();
    }
}