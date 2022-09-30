using CommonLib.Components;
using System;

namespace ComponentCheckApp.Views.DeviceSelect
{
    public class DeviceSelectUIControlsModel : ObservableProperty
    {
        public bool SelectorComboBoxIsEnable
        {
            get => selectorComboBoxIsEnable;
            private set => SetField(value, ref selectorComboBoxIsEnable);
        }

        public bool ScanButtonIsEnable
        {
            get => scanButtonIsEnable;
            private set => SetField(value, ref scanButtonIsEnable);
        }

        public bool ConnectButtonIsEnable
        {
            get => connectButtonIsEnable;
            private set => SetField(value, ref connectButtonIsEnable);
        }

        public bool SettingsButtonIsEnable
        {
            get => settingsButtonIsEnable;
            private set => SetField(value, ref settingsButtonIsEnable);
        }

        public bool DisconnectButtonIsEnable
        {
            get => disconnectButtonIsEnable;
            private set => SetField(value, ref disconnectButtonIsEnable);
        }


        bool selectorComboBoxIsEnable;
        bool scanButtonIsEnable;
        bool connectButtonIsEnable;
        bool settingsButtonIsEnable;
        bool disconnectButtonIsEnable;


        public void SetUIControlsByStatus(DeviceSelectModelStatus status)
        {
            switch(status)
            {
                case DeviceSelectModelStatus.Work:
                    SelectorComboBoxIsEnable = false;
                    ScanButtonIsEnable = false;
                    ConnectButtonIsEnable = false;
                    SettingsButtonIsEnable = false;
                    DisconnectButtonIsEnable = false;
                    break;

                case DeviceSelectModelStatus.DeviceAvailable:
                    SelectorComboBoxIsEnable = true;
                    ScanButtonIsEnable = true;
                    ConnectButtonIsEnable = true;
                    SettingsButtonIsEnable = false;
                    DisconnectButtonIsEnable = false;
                    break;

                case DeviceSelectModelStatus.DeviceNotAvailable:
                    SelectorComboBoxIsEnable = false;
                    ScanButtonIsEnable = true;
                    ConnectButtonIsEnable = false;
                    SettingsButtonIsEnable = false;
                    DisconnectButtonIsEnable = false;
                    break;

                case DeviceSelectModelStatus.DeviceConnected:
                    SelectorComboBoxIsEnable = false;
                    ScanButtonIsEnable = false;
                    ConnectButtonIsEnable = false;
                    SettingsButtonIsEnable = true;
                    DisconnectButtonIsEnable = true;
                    break;

                case DeviceSelectModelStatus.DeviceDisconnected:
                    SelectorComboBoxIsEnable = true;
                    ScanButtonIsEnable = true;
                    ConnectButtonIsEnable = true;
                    SettingsButtonIsEnable = false;
                    DisconnectButtonIsEnable = false;
                    break;
            }
        }
    }
}
