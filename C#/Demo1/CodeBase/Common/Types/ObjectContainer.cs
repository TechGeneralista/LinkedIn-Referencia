using System;


namespace Common.Types
{
    public class Container<T>
    {
        public event EventHandler ValueChanged;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        T value;
    }
}
