using Common;
using Common.Types;
using System;


namespace VisualBlocks.Module.Base
{
    internal class BlockItemDataOutputDC<T> : DCBase
    {
        public event EventHandler PullAction;

        public T Value
        {
            get
            {
                Active = true;
                PullAction?.Invoke(this, EventArgs.Empty);

                T retValue = value;
                value = default;

                return retValue;
            }

            set => this.value = value;
        }
        T value;

        public bool Active
        {
            get => active;
            set => SetField(ref active, value);
        }
        bool active;

        public BlockItemDataOutputDC(T initialValue = default)
            => value = initialValue;
    }
}
