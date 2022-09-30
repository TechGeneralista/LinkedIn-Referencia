using Common;
using ImageCaptureDevice.UniversalVideoClass.DirectShow;


namespace ImageCaptureDevice.UniversalVideoClass.CamVidProperties
{
    public class PropertyBase : DCBase
    {
        public string OriginalName { get; protected set; }
        public bool IsSupported { get; protected set; }
        public bool IsAutoManual { get; protected set; }
        public int MinValue { get; protected set; }
        public int DefaultValue { get; protected set; }
        public int MaxValue { get; protected set; }
        public int StepSize { get; protected set; }

        public bool IsAuto
        {
            get => isAuto;

            set
            {
                SetField(ref isAuto, value);
                SendToCamera();
            }
        }
        bool isAuto;

        public int Current 
        {
            get => current;

            set 
            { 
                SetField(ref current, value);
                SendToCamera();
            }
        }
        int current;


        protected VideoCaptureDevice videoCaptureDevice;


        public void SetToDefault()
        {
            if (!IsSupported)
                return;

            if (IsAutoManual)
                IsAuto = true;

            Current = DefaultValue;
        }

        protected virtual void SendToCamera() { }
    }
}
