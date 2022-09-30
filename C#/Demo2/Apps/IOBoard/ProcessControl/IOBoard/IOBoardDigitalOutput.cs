using Common.Prop;
using Common.Tool;
using System;


namespace IOBoard
{
    public class IOBoardDigitalOutput
    {
        public int Index { get; }
        public INonSettableObservableProperty<bool?> LastOutputState { get; } = new ObservableProperty<bool?>();
        public INonSettableObservableProperty<bool?> LastOutputTriggerRisingState { get; } = new ObservableProperty<bool?>();
        public INonSettableObservableProperty<bool?> LastOutputTriggerFallingState { get; } = new ObservableProperty<bool?>();


        Func<string, string> sendAndReceive;


        public IOBoardDigitalOutput(int index, Func<string, string> sendAndReceive)
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
            string response = sendAndReceive("gdo" + Index);

            if (response.IsNull() || response == "e" || response == "enc")
                LastOutputState.ForceSet(null);
            else
                LastOutputState.ForceSet(response == "h" ? true : false);

            return LastOutputState.Value;
        }

        public bool? Read(DigitalTriggers trigger)
        {
            string response = null;

            if (trigger == DigitalTriggers.Rising)
                response = sendAndReceive("gdor" + Index);
            else if (trigger == DigitalTriggers.Falling)
                response = sendAndReceive("gdof" + Index);

            if (response.IsNull() || response == "e" || response == "enc")
            {
                if (trigger == DigitalTriggers.Rising)
                    LastOutputTriggerRisingState.ForceSet(null);
                else if (trigger == DigitalTriggers.Falling)
                    LastOutputTriggerFallingState.ForceSet(null);

                return null;
            }
            else
            {
                bool b = response == "h" ? true : false;

                if (trigger == DigitalTriggers.Rising)
                    LastOutputTriggerRisingState.ForceSet(b);
                else if (trigger == DigitalTriggers.Falling)
                    LastOutputTriggerFallingState.ForceSet(b);

                return b;
            }
        }

        public bool? Write(bool logicLevel)
        {
            string response = sendAndReceive("sdo" + Index + (logicLevel ? "h" : "l"));

            if (response.IsNull() || response == "e" || response == "enc")
                LastOutputState.ForceSet(null);
            else if (response == "o")
                LastOutputState.ForceSet(logicLevel);

            return LastOutputState.Value;
        }

        public bool? Toggle()
        {
            bool? currentLevel = Read();

            if (currentLevel.IsNull())
                return null;

            return Write(!(bool)currentLevel);
        }
    }
}