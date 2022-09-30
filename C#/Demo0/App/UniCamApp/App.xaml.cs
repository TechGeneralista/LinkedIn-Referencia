using Common;
using Common.Settings;
using CustomControl.PopupWindow;
using Language;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System.Diagnostics;
using System.Windows;
using UniCamApp.Startup;


namespace UniCamApp
{
    public partial class App : Application
    {
        readonly string settingsFileExtension = @"ucc";
        LanguageDC languageDC;
        ISettingsCollection settingsCollection;
        OpenCLAccelerator openCLAccelerator;


        protected override void OnStartup(StartupEventArgs e)
        {
            int appInstanceIndex = Process.GetProcessesByName(nameof(UniCamApp)).Length;

            if (appInstanceIndex != 1)
            {
                Shutdown(-1);
                return;
            }

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            DLLFiles dllFiles = new DLLFiles();
            dllFiles.AddFileName(nameof(Common));
            dllFiles.AddFileName(nameof(Language));
            dllFiles.AddFileName(nameof(AppLog));
            dllFiles.AddFileName(nameof(OpenCLWrapper));
            dllFiles.AddFileName(nameof(CustomControl));
            dllFiles.AddFileName(nameof(ImageSourceDevice));
            dllFiles.AddFileName(nameof(ImageProcess));
            dllFiles.AddFileName(nameof(Communication));
            dllFiles.AddFileName(nameof(LogicalEvaluator));

            if(!dllFiles.CheckExistence())
            {
                Shutdown(-1);
                return;
            }

            string settingsFilePath = Utils.GetPath(string.Format("config.{0}", settingsFileExtension));
            ISettingsStore settingsStore = new FileSettingsStore(settingsFilePath);
            settingsCollection = new SettingsCollection(settingsStore, true);

            languageDC = new LanguageDC(settingsCollection);

            openCLAccelerator = new OpenCLAccelerator(languageDC);

            if (!openCLAccelerator.ScanDevice(DeviceTypePriority.GPUCPU))
            {
                Shutdown(-1);
                return;
            }

            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new AppWindow();
            MainWindow.Closing += (s, a) => SaveSettings();
            PopupWindowDC popupWindowDC = new PopupWindowDC(languageDC, MainWindow);
            AppDC appDC = new AppDC(settingsCollection, languageDC, openCLAccelerator, popupWindowDC);
            MainWindow.DataContext = appDC;
            MainWindow.Show();

            appDC.SourceDC.SettingsDC.AutoLoaderDC.StartAutoLoad();
        }

        private void SaveSettings()
        {
            settingsCollection.Write();

            if (settingsCollection.IsErrorOccured)
                MessageBox.Show(languageDC.SaveSettings.CurrentValue, languageDC.ErrorColon.CurrentValue, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
