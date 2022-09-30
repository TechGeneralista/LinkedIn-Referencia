using SmartVisionClientApp.Common;
using SmartVisionClientApp.Communication;
using SmartVisionClientApp.DTOs;
using System.Diagnostics;

namespace SmartVisionClientApp.CameraSelect
{
    public class CameraProperty
    {
        public string Name { get; }
        public double MinValue { get; }
        public double StepSize { get; }
        public double MaxValue { get; }
        public double DefaultValue { get; }
        public ISettableObservableProperty<double> Current { get; } = new ObservableProperty<double>();


        TCPClient client;
        int camIndex;


        public CameraProperty(int camIndex, TCPClient client, CamProperties cp)
        {
            this.camIndex = camIndex;
            this.client = client;

            Name = cp.Property;
            MinValue = cp.PropertyInfo.MinValue;
            StepSize = cp.PropertyInfo.StepCount;
            MaxValue = cp.PropertyInfo.MaxValue;
            DefaultValue = cp.PropertyInfo.CurrentValue;
            Current.Value = cp.PropertyInfo.CurrentValue;

            Current.ValueChanged += Current_ValueChanged;
        }

        private void Current_ValueChanged(double newValue)
        {
            QueryDTO queryDTO = new QueryDTO()
            {
                Query = "CAMERA_SETPROP",
                Params = new CamProp()
                {
                    CameraIndex = camIndex,
                    Name = Name,
                    Value = newValue
                }
            };

            client.SendAndReceive<QueryDTO, QueryInfoResponse>(queryDTO);
        }
    }
}