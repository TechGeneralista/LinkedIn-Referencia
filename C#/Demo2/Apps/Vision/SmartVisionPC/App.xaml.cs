using SmartVisionClientApp.CameraSelect;
using SmartVisionClientApp.Common;
using SmartVisionClientApp.Communication;
using SmartVisionClientApp.Trigger;
using System;
using System.Net;
using System.Windows;


namespace SmartVisionClientApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IPAddress serverAddr;
            int serverPort;

            try
            {
                CommandLineParser commandLineParser = new CommandLineParser(e.Args);
                serverAddr = IPAddress.Parse(commandLineParser.GetParameter("tcpaddr"));
                serverPort = int.Parse(commandLineParser.GetParameter("tcpport"));
            }
            catch (Exception ex)
            {
                Utils.ShowErrorAndShutdown("Hiba a paraméterek olvasása közben", ex);
                return;
            }

            try
            {
                ObjectContainer.Set(new TCPClient(serverAddr, serverPort));
            }
            catch (Exception ex)
            {
                Utils.ShowErrorAndShutdown("Nem sikerült kapcsolódni a szerverhez", ex);
                return;
            }

            ObjectContainer.Set(new CameraSelectViewModel());
            ObjectContainer.Set(new CameraSelectView());

            ObjectContainer.Set(new TriggerViewModel());

            ObjectContainer.Set(new MainWindowViewModel());
            ObjectContainer.Set(new MainWindow());

            MainWindow = ObjectContainer.Get<MainWindow>();
            MainWindow.Closing += MainWindow_Closing;
            MainWindow.Show();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ObjectContainer.Get<TriggerViewModel>().StopAndWaitForStop();
            ObjectContainer.Get<CameraSelectViewModel>().DisconnectButtonClick();
        }
    }
}
