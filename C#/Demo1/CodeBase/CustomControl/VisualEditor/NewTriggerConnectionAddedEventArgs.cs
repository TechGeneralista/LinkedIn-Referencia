using System;


namespace CustomControl.VisualEditor
{
    public class NewTriggerConnectionAddedEventArgs : EventArgs
    {
        public object SourceOutput { get; }
        public object DestinationInput { get; }

        public NewTriggerConnectionAddedEventArgs(object sourceOutput, object destinationInput)
        {
            SourceOutput = sourceOutput;
            DestinationInput = destinationInput;
        }
    }
}
