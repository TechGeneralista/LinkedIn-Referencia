using System.Collections;


namespace CustomControl.VisualEditor
{
    public class RemoveItemsEventArgs
    {
        public IEnumerable ItemsToRemove { get; }

        public RemoveItemsEventArgs(IEnumerable items)
        {
            ItemsToRemove = items;
        }
    }
}