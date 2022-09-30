using MultiCamApp.MultiCam;
using System;

namespace MultiCamApp.Connection
{
    public class ConnectionViewModel
    {
        public GuiControls GuiControls { get; }
        public MultiCamDeviceScanner MultiCamDeviceScanner { get; }


        public ConnectionViewModel()
        {
            GuiControls = new GuiControls();
            MultiCamDeviceScanner = new MultiCamDeviceScanner();

            GuiControls.Disconnect(null);
        }

        internal async void ScanButtonClick()
        {
            GuiControls.DisableAll();
            await MultiCamDeviceScanner.ScanAsync();
            GuiControls.Disconnect(null);
        }
    }
}
