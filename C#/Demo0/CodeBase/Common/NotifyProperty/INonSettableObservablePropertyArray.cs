using System;


namespace Common.NotifyProperty
{
    public interface INonSettableObservablePropertyArray<T>
    {
        event Action<T[],T[]> CurrentValueChanged;

        T[] PreviousValue { get; }
        T[] CurrentValue { get; }


        bool Contains(T item);
        void ForceAdd(T item);
        void ForceAddRange(T[] items);
        void ForceRemove(T item);
        void ForceClear();
    }
}