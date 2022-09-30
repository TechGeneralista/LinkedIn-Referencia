using Common;
using Common.Language;
using Common.Settings;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;


namespace UCVisionResultExplorerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly string applicationName = "UCVision Result Explorer";
        ISettingsCollection settingsCollection;
        LanguageDC languageDC;


        protected override void OnStartup(StartupEventArgs e)
        {
            int appInstanceIndex = Process.GetProcessesByName(nameof(UCVisionResultExplorerApp)).Length;

            if (appInstanceIndex != 1)
            {
                Shutdown(-1);
                return;
            }

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            DLLFiles dllFiles = new DLLFiles();
            dllFiles.AddFileName(nameof(Common));

            if (!dllFiles.CheckExistence())
            {
                Shutdown(-1);
                return;
            }

            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppInfo appInfo = new AppInfo(Constants.SmartSolutionsDebrecen, applicationName, Assembly.GetExecutingAssembly().GetName().Version);
            settingsCollection = new SettingsCollection(new FileSettingsStore(appInfo.ConfigFilePath), true);
            languageDC = new LanguageDC(settingsCollection);

            MainWindow = new MainWindow() { DataContext = new MainDC(settingsCollection, languageDC, appInfo) };
            MainWindow.Closed += MainWindow_Closed;
            MainWindow.Show();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            settingsCollection.Write();

            if (settingsCollection.SettingsStore.IsErrorOccured)
                MessageBox.Show(languageDC.SaveSettings.Value, languageDC.ErrorColon.Value, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
