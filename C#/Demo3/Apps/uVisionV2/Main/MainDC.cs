using Common;
using Common.Language;
using Common.ObservableProperty;
using Common.Settings;
using System;
using System.Windows;
using uVisionV2.Modules.TriggerButton;
using uVisionV2.Settings;


namespace uVisionV2.Main
{
    public class MainDC
    {
        public AppInfo AppInfo { get; }
        public LanguageDC LanguageDC { get; }
        public IObservableCollection<object> Modules { get; } = new ObservableCollection<object>();


        readonly Window mainWindow;
        readonly SettingsDC settingsDC;


        public MainDC(AppInfo appInfo, ISettingsCollection settingsCollection, LanguageDC languageDC, Window mainWindow)
        {
            AppInfo = appInfo;
            LanguageDC = languageDC;
            this.mainWindow = mainWindow;

            settingsDC = new SettingsDC(languageDC);
        }

        internal void AddTriggerButtonClick()
            => new TriggerButtonDC(LanguageDC, Modules);

        internal void SettingsButtonClick()
        {
            SettingsV settingsV = new SettingsV();
            settingsV.DataContext = settingsDC;
            settingsV.Owner = mainWindow;
            settingsV.ShowDialog();
        }
    }
}
