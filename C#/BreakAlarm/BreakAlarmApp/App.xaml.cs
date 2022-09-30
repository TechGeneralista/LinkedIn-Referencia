using System.Windows;


namespace BreakAlarmApp
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainDC mainDC = new MainDC();

            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = mainDC;

            MainWindow = mainWindow;
            MainWindow.Closing += MainWindow_Closing;
            MainWindow.Show();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(MainWindow.DataContext is MainDC mainDC)
            {
                mainDC.Write();
            }
        }
    }
}
