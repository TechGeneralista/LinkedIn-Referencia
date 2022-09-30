using Common;
using System;


namespace VisualBlocks.Module.Base
{
    internal class BlockItemTriggerOutputDC : DCBase
    {
        public bool Active
        {
            get => active;
            set => SetField(ref active, value);
        }
        bool active;


        readonly DependencyParams dependencyParams;


        public BlockItemTriggerOutputDC(DependencyParams dependencyParams)
        {
            this.dependencyParams = dependencyParams;
        }

        internal void Trigger(object token = null)
        {
            Active = true;
            BlockItemTriggerInputDC blockItemTriggerInputDC = null;

            lock (dependencyParams.Items.Lock)
            {
                foreach (object item in dependencyParams.Items)
                {
                    if (item is BlockItemTriggerConnectionDC blockItemTriggerConnectionDC &&
                       blockItemTriggerConnectionDC.BlockItemTriggerOutputDC == this)
                    {
                        blockItemTriggerConnectionDC.Active = true;
                        blockItemTriggerInputDC = blockItemTriggerConnectionDC.BlockItemTriggerInputDC;
                        break;
                    }
                }
            }

            if(blockItemTriggerInputDC != null)
            {
                if (token == null)
                    token = new object();

                blockItemTriggerInputDC.Trigger(token);
            }
        }
    }
}
