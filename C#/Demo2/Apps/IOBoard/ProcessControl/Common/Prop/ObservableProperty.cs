﻿using System;

namespace Common.Prop
{
    public class ObservableProperty<T> : PropertyBase, ISettableObservableProperty<T>, INonSettableObservableProperty<T>
    {
        public event Action<T> ValueChanged;

        public T Value 
        {
            get => v;
            set
            {
                SetField(value, ref v);
                ValueChanged?.Invoke(v);
            }
        }

        T v;

        public ObservableProperty() => v = default;
        public ObservableProperty(T initialValue) => v = initialValue;

        public void ForceSet(T newValue) => Value = newValue;
    }
}
