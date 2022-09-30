using System.Windows;
using ViProEditorApp.UI.Main;

namespace ViProEditorApp
{
    public partial class App : Application
    {
        public App()
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainView();
            MainWindow.DataContext = new MainViewModel();
            MainWindow.Show();
        }
    }
}
