using UVCLib.DirectShow;
using System;
using CommonLib.Components;


namespace UVCLib.Components.Internals
{
    [Flags]
    public enum ControlFlags { None = 0, Auto, Manual }

    public class PropertyBase : ObservableProperty
    {
        public int MinValue { get; protected set; }
        public int DefaultValue { get; protected set; }
        public int MaxValue { get; protected set; }
        public int StepSize { get; protected set; }


        protected VideoCaptureDevice videoCaptureDevice;

        protected ControlFlags RepairSupportedControlModes(ControlFlags supportedControlModes)
        {
            if (supportedControlModes == (ControlFlags.Auto | ControlFlags.Manual))
                return ControlFlags.Auto | ControlFlags.Manual;

            else if (supportedControlModes == ControlFlags.Auto)
                return ControlFlags.Auto;

            else if (supportedControlModes == ControlFlags.Manual)
                return ControlFlags.Manual;

            return ControlFlags.None;
        }

        protected ControlFlags RepairCurrentControlMode(ControlFlags currentControlMode)
        {
            if (currentControlMode == (ControlFlags.Auto | ControlFlags.Manual) || currentControlMode == ControlFlags.Auto)
                return ControlFlags.Auto;

            else if (currentControlMode == ControlFlags.Manual)
                return ControlFlags.Manual;

            return ControlFlags.None;
        }
    }
}
