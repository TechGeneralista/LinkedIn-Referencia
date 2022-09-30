using Common;
using System;


namespace VisualBlocks.Module.Base
{
    internal class BlockItemTriggerInputDC : DCBase
    {
        public event EventHandler<TriggerActionArgs> TriggerAction;

        public bool Connected 
        {
            get
            {
                lock(dependencyParams.Items.Lock)
                {
                    foreach (object item in dependencyParams.Items)
                    {
                        if (item is BlockItemTriggerConnectionDC blockItemTriggerConnectionDC &&
                            blockItemTriggerConnectionDC.BlockItemTriggerInputDC == this)
                            return true;
                    }

                    return false;
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
        object lastToken;


        public BlockItemTriggerInputDC(DependencyParams dependencyParams)
        {
            this.dependencyParams = dependencyParams;
        }

        internal void Trigger(object token)
        {
            if(token != lastToken)
            {
                Active = true;
                lastToken = token;
                TriggerAction?.Invoke(this, new TriggerActionArgs(token));
            }
        }
    }
}
