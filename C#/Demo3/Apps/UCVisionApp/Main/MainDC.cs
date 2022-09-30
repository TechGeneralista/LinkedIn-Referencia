using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.PopupWindow;
using System.Windows;
using Common;
using Common.Interfaces;
using UCVisionApp.ImageSourceDevices;
using OpenCLWrapper;
using System.Windows.Controls;
using UCVisionApp.Settings;
using Common.Communication.SimpleTCP.Server.MultiClient;
using Common.Settings;
using Common.SaveResult;
using Common.License;


namespace UCVisionApp.Main
{
    internal class MainDC : ICanRemote, ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public LogDC LogDC { get; }
        public IReadOnlyProperty<string> TitleText { get; }
        public IReadOnlyPropertyArray<object> Children { get; } = new PropertyArray<object>();
        public IReadOnlyProperty<object> CurrentContent { get; } = new Property<object>();


        readonly SaveResultDC saveResultDC;
        readonly AppInfo appInfo;


        public MainDC(LanguageDC languageDC, LogDC logDC, OpenCLAccelerator openCLAccelerator, PopupWindowDC popupWindowDC, TCPServerMultiClientDC tcpServerMultiClientDC, ISettingsCollection settingsCollection, IReadOnlyProperty<string> titleText, AppInfo appInfo)
        {
            LanguageDC = languageDC;
            LogDC = logDC;
            TitleText = titleText;
            this.appInfo = appInfo;

            saveResultDC = new SaveResultDC(languageDC);

            Children.ToSettable().Add(new ImageSourceDevicesDC(languageDC, logDC, openCLAccelerator, popupWindowDC, saveResultDC));
            Children.ToSettable().Add(new SettingsDC(languageDC, tcpServerMultiClientDC, logDC, popupWindowDC, this, Children.Value[0].CastTo<ICanStopContinousTrigger>(), Children.Value[0].CastTo<ICanShootAsync>(), settingsCollection, saveResultDC));
        }

        internal void TreeView_Loaded(TreeView treeView, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = treeView.ItemContainerGenerator.ContainerFromItem(Children.Value[0]) as TreeViewItem;

            if (treeViewItem.IsNotNull())
                treeViewItem.IsSelected = true;
        }

        internal void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            e.OldValue.CastTo<ICanResetSelectedTabItemIndex>()?.ResetSelectedTabItemIndex();
            
            IHasIsSelected hasIsSelected = e.OldValue.CastTo<IHasIsSelected>();

            if(hasIsSelected.IsNotNull())
                hasIsSelected.IsSelected.Value = false;

            hasIsSelected = e.NewValue.CastTo<IHasIsSelected>();

            if(hasIsSelected.IsNotNull())
                hasIsSelected.IsSelected.Value = true;

            CurrentContent.ToSettable().Value = e.NewValue;
        }

        public string Remote(string command, string[] ids)
        {
            string response = null;
            response = Children.Value[0].CastTo<ICanRemote>().Remote(command, ids);
            return response;
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            settingsCollection.SetProperty(appInfo.ApplicationNameVersion, nameof(appInfo.ApplicationNameVersion));
            saveResultDC.SaveSettings(settingsCollection);

            Children.Value[0].CastTo<ICanSaveLoadSettings>().SaveSettings(settingsCollection);

            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            string title = settingsCollection.GetProperty<string>(nameof(appInfo.ApplicationNameVersion));

            //if(Title != title)
            //{
            //    settingsCollection.ErrorOccurred();
            //    LogDC.NewMessage(LogTypes.Error, LanguageDC.FileVersionColon, title);
            //    return;
            //}

            saveResultDC.LoadSettings(settingsCollection);
            Children.Value[0].CastTo<ICanSaveLoadSettings>().LoadSettings(settingsCollection);

            settingsCollection.ExitPoint();
        }
    }
}