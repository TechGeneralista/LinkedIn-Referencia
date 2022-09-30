using System.Threading;
using System.Threading.Tasks;
using UVCLib.DirectShow;


namespace UVCLib.Components.Internals
{
    public enum CameraControlProperties { Pan = 0, Tilt, Roll, Zoom, Exposure, Iris, Focus }


    public class CameraControlProperty : PropertyBase
    {
        public bool IsSupported
        {
            get => isSupported;
            private set => SetField(value, ref isSupported);
        }

        public bool AutoManualSwitchIsEnable
        {
            get => autoManualSwitchIsEnable;
            private set => SetField(value, ref autoManualSwitchIsEnable);
        }

        public bool IsAuto
        {
            get => isAuto;
            set
            {
                SetField(value, ref isAuto);
                IsAutoChanged();
            }
        }

        public bool SliderIsEnable
        {
            get => sliderIsEnable;
            private set => SetField(value, ref sliderIsEnable);
        }

        public int CurrentValue
        {
            get => currentValue;
            set
            {
                SetField(value, ref currentValue);
                SetCurrentValue();
            }
        }


        bool sliderIsEnable;
        int currentValue;
        bool isAuto;
        bool autoManualSwitchIsEnable;
        bool isSupported;
        CameraControlProperties propertyName;
        ControlFlags currentControlMode;
        ControlFlags supportedControlModes;
        bool currentValueGetterIsRun;


        public CameraControlProperty(VideoCaptureDevice videoCaptureDevice, CameraControlProperties propertyName)
        {
            base.videoCaptureDevice = videoCaptureDevice;
            this.propertyName = propertyName;

            try
            {
                GetCameraPropertyRange();

                if (supportedControlModes != ControlFlags.None)
                {
                    isSupported = true;

                    if (supportedControlModes == (ControlFlags.Auto | ControlFlags.Manual))
                        autoManualSwitchIsEnable = true;
                    else
                        autoManualSwitchIsEnable = false;

                    GetCurrentValueAndCurrentControlMode();

                    if (currentControlMode == ControlFlags.Auto)
                    {
                        isAuto = true;
                        sliderIsEnable = false;
                        Task.Run(() => GetCurrentValueAuto());
                    }
                    else if (currentControlMode == ControlFlags.Manual)
                    {
                        isAuto = false;
                        sliderIsEnable = true;
                    }
                }
            }
            catch { }
        }

        private void GetCameraPropertyRange()
        {
            videoCaptureDevice.GetCameraPropertyRange
            (
                propertyName,
                out int minValue,
                out int maxValue,
                out int stepSize,
                out int defaultValue,
                out ControlFlags supportedControlModes
            );

            supportedControlModes = RepairSupportedControlModes(supportedControlModes);
            this.supportedControlModes = supportedControlModes;

            if(supportedControlModes != ControlFlags.None)
            {
                MinValue = minValue;
                MaxValue = maxValue;
                StepSize = stepSize;
                DefaultValue = defaultValue;
            }
        }

        public void SetDefaultValue()
        {
            CurrentValue = DefaultValue;

            if (supportedControlModes == (ControlFlags.Auto | ControlFlags.Manual) || supportedControlModes == ControlFlags.Auto)
                IsAuto = true;

            else if (supportedControlModes == ControlFlags.Manual)
                IsAuto = false;
        }

        private void GetCurrentValueAndCurrentControlMode()
        {
            videoCaptureDevice.GetCameraProperty
            (
                propertyName,
                out int currentValue,
                out ControlFlags currentControlMode
            );

            currentControlMode = RepairCurrentControlMode(currentControlMode);

            this.currentControlMode = currentControlMode;
            this.currentValue = currentValue;
        }

        private void IsAutoChanged()
        {
            GetCurrentValueAndCurrentControlMode();
            CurrentValue = currentValue;

            if (isAuto)
            {
                SetCurrentControlModeAuto();
                SliderIsEnable = false;
                Task.Run(()=>GetCurrentValueAuto());
            }
            else
            {
                SetCurrentControlModeManual();
                SliderIsEnable = true;
            }
        }

        private void SetCurrentControlModeManual()
        {
            currentControlMode = ControlFlags.Manual;
            videoCaptureDevice.SetCameraProperty(propertyName, currentValue, currentControlMode);
        }

        private void SetCurrentControlModeAuto()
        {
            currentControlMode = ControlFlags.Auto;
            videoCaptureDevice.SetCameraProperty(propertyName, currentValue, currentControlMode);
        }

        private void SetCurrentValue()
        {
            videoCaptureDevice.SetCameraProperty(propertyName, currentValue, currentControlMode);
        }

        private void GetCurrentValueAuto()
        {
            if (currentValueGetterIsRun)
                return;

            currentValueGetterIsRun = true;

            while (isAuto)
            {
                videoCaptureDevice.GetCameraProperty
                (
                    propertyName,
                    out int currentValue,
                    out ControlFlags currentControlMode
                );

                CurrentValue = currentValue;

                Thread.Sleep(1000);
            }

            currentValueGetterIsRun = false;
        }
    }
}
