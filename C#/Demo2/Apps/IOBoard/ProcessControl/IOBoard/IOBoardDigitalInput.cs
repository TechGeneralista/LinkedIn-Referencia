using Common.Prop;
using Common.Tool;
using System;


namespace IOBoard
{
    public class IOBoardDigitalInput
    {
        public int Index { get; }
        public INonSettableObservableProperty<bool?> LastInputState { get; } = new ObservableProperty<bool?>();
        public INonSettableObservableProperty<bool?> LastInputTriggerRisingState { get; } = new ObservableProperty<bool?>();
        public INonSettableObservableProperty<bool?> LastInputTriggerFallingState { get; } = new ObservableProperty<bool?>();


        Func<string, string> sendAndReceive;


        public IOBoardDigitalInput(int index, Func<string, string> sendAndReceive)
        {
            Index = index;
            this.sendAndReceive = sendAndReceive;

            Refresh();
        }

        public void Refresh()
        {
            Read();
            Read(DigitalTriggers.Rising);
            Read(DigitalTriggers.Falling);
        }

        public bool? Read()
        {
            string response = sendAndReceive("gdi" + Index);

            if(response.IsNull() || response == "e" || response == "enc")
                LastInputState.ForceSet(null);
            else
                LastInputState.ForceSet(response == "h" ? true : false);

            return LastInputState.Value;
        }

        public bool? Read(DigitalTriggers trigger)
        {
            string response = null;

            if (trigger == DigitalTriggers.Rising)
                response = sendAndReceive("gdir" + Index);
            else if (trigger == DigitalTriggers.Falling)
                response = sendAndReceive("gdif" + Index);

            if (response.IsNull() || response == "e" || response == "enc")
            {
                if(trigger == DigitalTriggers.Rising)
                    LastInputTriggerRisingState.ForceSet(null);
                else if (trigger == DigitalTriggers.Falling)
                    LastInputTriggerFallingState.ForceSet(null);

                return null;
            }
            else
            {
                bool b = response == "h" ? true : false;

                if (trigger == DigitalTriggers.Rising)
                    LastInputTriggerRisingState.ForceSet(b);
                else if (trigger == DigitalTriggers.Falling)
                    LastInputTriggerFallingState.ForceSet(b);

                return b;
            }
        }
    }
}