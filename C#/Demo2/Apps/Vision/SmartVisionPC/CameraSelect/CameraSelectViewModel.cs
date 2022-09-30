using SmartVisionClientApp.Common;
using SmartVisionClientApp.Communication;
using SmartVisionClientApp.DTOs;
using SmartVisionClientApp.Trigger;


namespace SmartVisionClientApp.CameraSelect
{
    public class CameraSelectViewModel
    {
        public ISettableObservableProperty<ResponseObject[]> AvailableCameras { get; set; } = new ObservableProperty<ResponseObject[]>();
        public ISettableObservableProperty<ResponseObject> SelectedCamera { get; set; } = new ObservableProperty<ResponseObject>();
        public ISettableObservableProperty<bool> AvailableCamerasListIsEnable { get; set; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<bool> RefreshAvailableCamerasButtonIsEnable { get; set; } = new ObservableProperty<bool>(true);
        public ISettableObservableProperty<bool> ConnectButtonIsEnable { get; set; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<bool> DisconnectButtonIsEnable { get; set; } = new ObservableProperty<bool>();


        public CameraSelectViewModel()
        {
            SelectedCamera.ValueChanged += SelectedCamera_ValueChanged;
        }

        private void SelectedCamera_ValueChanged(ResponseObject obj)
        {
            if (obj.IsNotNull())
                ConnectButtonIsEnable.Value = true;
            else
                ConnectButtonIsEnable.Value = false;
        }

        internal void RefreshAvailableCamerasButtonClick()
        {
            DisableAllControl();

            TCPClient client = ObjectContainer.Get<TCPClient>();

            QueryDTO queryDTO = new QueryDTO()
            {
                Query = "CAMERA_AVAILCAMS",
                Params = new object()
            };

            AvailableCamerasResponse serverResponse = client.SendAndReceive<QueryDTO, AvailableCamerasResponse>(queryDTO);

            if (serverResponse.IsNotNull())
            {
                AvailableCamerasListIsEnable.Value = true;

                SelectedCamera.Value = null;
                AvailableCameras.Value = serverResponse.Response.ResponseObject;

                if (AvailableCameras.Value.Length > 0)
                    AvailableCamerasListIsEnable.Value = true;
                else
                    AvailableCamerasListIsEnable.Value = false;

                if (AvailableCameras.Value.Length == 1)
                    SelectedCamera.Value = AvailableCameras.Value[0];
            }

            RefreshAvailableCamerasButtonIsEnable.Value = true;
        }

        internal void ConnectButtonClick()
        {
            DisableAllControl();

            TCPClient client = ObjectContainer.Get<TCPClient>();

            QueryDTO queryDTO = new QueryDTO()
            {
                Query = "CAMERA_OPENCAM",
                Params = new CameraIndexParams() { CameraIndex = SelectedCamera.Value.DevIdx }
            };

            ConnectCameraResponse serverResponse = client.SendAndReceive<QueryDTO, ConnectCameraResponse>(queryDTO);

            if (serverResponse.IsNotNull())
            {
                DisconnectButtonIsEnable.Value = true;
                ObjectContainer.Get<MainWindowViewModel>().ImageOptimizationButtonIsEnable.Value = true;
                ObjectContainer.Set(new Camera(client, serverResponse));

                Camera connectedCamera = ObjectContainer.Get<Camera>();

                TriggerViewModel triggerViewModel = ObjectContainer.Get<TriggerViewModel>();
                triggerViewModel.SingleStartButtonIsEnable.Value = true;
                triggerViewModel.SourceComboBoxIsEnable.Value = true;
                triggerViewModel.SingleStartButtonClick();
            }
            else
            {
                AvailableCamerasListIsEnable.Value = true;
                RefreshAvailableCamerasButtonIsEnable.Value = true;
                ConnectButtonIsEnable.Value = true;
            }
        }

        private void DisableAllControl()
        {
            AvailableCamerasListIsEnable.Value = false;
            RefreshAvailableCamerasButtonIsEnable.Value = false;
            ConnectButtonIsEnable.Value = false;
            DisconnectButtonIsEnable.Value = false;
        }

        internal async void DisconnectButtonClick()
        {
            Camera connectedCamera = ObjectContainer.Get<Camera>();

            if(connectedCamera.IsNotNull())
            {
                DisableAllControl();

                TriggerViewModel triggerViewModel = ObjectContainer.Get<TriggerViewModel>();
                await triggerViewModel.StopAndWaitForStopAsync();
                triggerViewModel.SingleStartButtonIsEnable.Value = false;
                triggerViewModel.SourceComboBoxIsEnable.Value = false;

                if (connectedCamera.Disconnect())
                {
                    AvailableCamerasListIsEnable.Value = true;
                    RefreshAvailableCamerasButtonIsEnable.Value = true;
                    ConnectButtonIsEnable.Value = true;
                    ObjectContainer.Get<MainWindowViewModel>().ImageOptimizationButtonIsEnable.Value = false;

                    ObjectContainer.Set<Camera>(null);
                    ObjectContainer.Get<MainWindowViewModel>().CurrentImage.Value = Utils.GetBlackImage();
                }
                else
                {
                    DisconnectButtonIsEnable.Value = true;
                }
            }
        }
    }
}
