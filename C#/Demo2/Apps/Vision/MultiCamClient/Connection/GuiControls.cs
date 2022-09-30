namespace MultiCamApp.Connection
{
    public class GuiControls : ObservableProperty
    {
        public bool IsDeviceListEnable
        {
            get => isDeviceListEnable;
            private set => SetField(value, ref isDeviceListEnable);
        }

        public bool IsScanButtonEnable
        {
            get => isScanButtonEnable;
            private set => SetField(value, ref isScanButtonEnable);
        }

        public bool IsConnectButtonEnable
        {
            get => isConnectButtonEnable;
            private set => SetField(value, ref isConnectButtonEnable);
        }

        public bool IsDisconnectButtonEnable
        {
            get => isDisconnectButtonEnable;
            private set => SetField(value, ref isDisconnectButtonEnable);
        }


        bool isDeviceListEnable, isScanButtonEnable, isConnectButtonEnable, isDisconnectButtonEnable;


        public void Disconnect(object selectedDevice)
        {
            IsDeviceListEnable = true;
            IsScanButtonEnable = true;

            if (selectedDevice != null)
                IsConnectButtonEnable = true;
            else
                IsConnectButtonEnable = false;

            IsDisconnectButtonEnable = false;
        }

        public void DisableAll()
        {
            IsDeviceListEnable = false;
            IsScanButtonEnable = false;
            IsConnectButtonEnable = false;
            IsDisconnectButtonEnable = false;
        }
    }
}