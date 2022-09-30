using Common.Communication.SimpleTCP.Server.MultiClient;
using Common.Interfaces;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.PopupWindow;
using Common.SaveResult;
using Common.Settings;
using System;
using UCVisionApp.Settings.LoadSave;


namespace UCVisionApp.Settings
{
    public class SettingsDC : IHasIsSelected, ICanStartAutoLoad
    {
        public LanguageDC LanguageDC { get; }
        public IProperty<bool> IsSelected { get; } = new Property<bool>();
        public TCPServerMultiClientDC TCPServerMultiClientDC { get; }
        public LoadSaveDC LoadSaveDC { get; }
        public SaveResultDC SaveResultDC { get; }


        public SettingsDC(LanguageDC languageDC, TCPServerMultiClientDC tcpServerMultiClientDC, LogDC logDC, PopupWindowDC popupWindowDC, ICanSaveLoadSettings canSaveLoadSettings, ICanStopContinousTrigger canStopContinousTrigger, ICanShootAsync canShootAsync, ISettingsCollection settingsCollection, SaveResultDC saveResultDC)
        {
            LanguageDC = languageDC;
            TCPServerMultiClientDC = tcpServerMultiClientDC;
            LoadSaveDC = new LoadSaveDC(languageDC, logDC, popupWindowDC, canSaveLoadSettings, canStopContinousTrigger, canShootAsync, settingsCollection);
            SaveResultDC = saveResultDC;

            IsSelected.OnValueChanged += IsSelected_OnValueChanged;
        }

        private async void IsSelected_OnValueChanged(bool o, bool n)
        {
            if (n)
            {
                await SaveResultDC.RefreshFreeSpaceBarAsync();
            }
        }

        public void StartAutoLoad()
            => LoadSaveDC.AutoLoaderDC.StartAutoLoad();
    }
}
