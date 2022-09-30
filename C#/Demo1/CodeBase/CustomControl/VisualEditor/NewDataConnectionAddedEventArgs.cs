using System;


namespace CustomControl.VisualEditor
{
    public class NewDataConnectionAddedEventArgs : EventArgs
    {
        public object SourceOutput { get; }
        public object DestinationInput { get; }

        public NewDataConnectionAddedEventArgs(object sourceOutput, object destinationInput)
        {
            SourceOutput = sourceOutput;
            DestinationInput = destinationInput;
        }
    }
}
