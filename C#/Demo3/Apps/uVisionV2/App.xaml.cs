using Common;
using Common.Language;
using Common.Settings;
using System.Reflection;
using System.Windows;
using uVisionV2.Main;


namespace uVisionV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly string appName = "uVision";


        protected override void OnStartup(StartupEventArgs e)
        {


            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppInfo appInfo = new AppInfo(Constants.SmartSolutionsDebrecen, appName, Assembly.GetExecutingAssembly().GetName().Version);
            ISettingsStore settingsStore = new FileSettingsStore(appInfo.ConfigFilePath);
            ISettingsCollection settingsCollection = new SettingsCollection(settingsStore);
            LanguageDC languageDC = new LanguageDC(settingsCollection);

            MainWindow = new MainV();
            MainWindow.Closed += (s, ee) => settingsCollection.Write();
            MainWindow.DataContext = new MainDC(appInfo, settingsCollection, languageDC, MainWindow);
            MainWindow.Show();
        }
    }
}
