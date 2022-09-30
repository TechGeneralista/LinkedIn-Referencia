using System;


namespace VisualBlocks.Module.Base
{
    internal class TriggerActionArgs : EventArgs
    {
        public object Token { get; }

        public TriggerActionArgs(object token)
        {
            Token = token;
        }
    }
}
