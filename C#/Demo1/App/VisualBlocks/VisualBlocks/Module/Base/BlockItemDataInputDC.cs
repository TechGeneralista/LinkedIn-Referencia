using Common;


namespace VisualBlocks.Module.Base
{
    internal class BlockItemDataInputDC<T> : DCBase
    {
        public T Value
        {
            get
            {
                Active = true;

                lock (dependencyParams.Items.Lock)
                {
                    foreach (object item in dependencyParams.Items)
                    {
                        if (item is BlockItemDataConnectionDC<T> blockItemDataConnectionDC &&
                            blockItemDataConnectionDC.BlockItemDataInputDC == this)
                        {
                            blockItemDataConnectionDC.Active = true;
                            return blockItemDataConnectionDC.BlockItemDataOutputDC.Value;
                        }
                    }

                    return default;
                }
            }
        }

        public bool Active
        {
            get => active;
            set => SetField(ref active, value);
        }
        bool active;


        readonly DependencyParams dependencyParams;


        public BlockItemDataInputDC(DependencyParams dependencyParams)
        {
            this.dependencyParams = dependencyParams;
        }
    }
}
