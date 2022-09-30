using Common.Language;
using System.ComponentModel;
using System.Windows;


namespace InstallerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LanguageDC languageDC = new LanguageDC(null);
            InstallerV installerV = new InstallerV();
            InstallerDC installerDC = new InstallerDC(languageDC, installerV);
            installerDC.Finished += InstallerDC_Finished;
            installerV.DataContext = installerDC;
            MainWindow = installerV;
            MainWindow.Closing += MainWindow_Closing;
            MainWindow.Show();
        }

        private void InstallerDC_Finished()
            => MainWindow.Close();

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            
        }
    }
}
