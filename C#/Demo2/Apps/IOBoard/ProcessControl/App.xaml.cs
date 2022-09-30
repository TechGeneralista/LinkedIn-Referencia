using Common.Log;
using Common.Tool;
using Editor;
using IOBoard;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using UserProgram;


namespace ProcessControlApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static readonly string configFileName = "config.cfg";


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ILog log = new FileLog();
            ObjectContainer.Set(log);
            Settings settings;

            try
            {
                settings = new Settings(configFileName);
            }
            catch
            {
                MessageBox.Show("Can't load config.cfg file", "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown(-1);
                return;
            }

            ObjectContainer.Set(new IOBoardClient(settings.ServerIPAddress, settings.ServerPort, log));
            ObjectContainer.Set(new EditorViewModel());
            ObjectContainer.Set(new InterpreterViewModel());
            ObjectContainer.Set(new MainViewModel());

            AutoStart(e.Args);

            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        private void AutoStart(string[] args)
        {
            string fileToLoadAndRun = null;

            if (args.Length != 0)
                fileToLoadAndRun = args[0];

            if (!string.IsNullOrEmpty(fileToLoadAndRun) && !string.IsNullOrWhiteSpace(fileToLoadAndRun))
            {
                int elapsedTime = 0;
                int interval = 100;

                IOBoardClient ioBoardClient = ObjectContainer.Get<IOBoardClient>();

                while (ioBoardClient.Devices.Value.IsNull())
                {
                    Thread.Sleep(100);
                    elapsedTime += interval;

                    if (elapsedTime == 3000)
                    {
                        MessageBox.Show("Nem lehet kapcsolódni a szerverhez", "Hiba:", MessageBoxButton.OK, MessageBoxImage.Error);
                        Current.Shutdown(-1);
                        return;
                    }
                }

                elapsedTime = 0;

                while (ioBoardClient.Devices.Value.Count() == 0)
                {
                    Thread.Sleep(100);
                    elapsedTime += interval;

                    if (elapsedTime == 3000)
                    {
                        MessageBox.Show("Nem lehet kapcsolódni a szerverhez vagy nincs csatlakoztatott IOBoard eszköz", "Hiba:", MessageBoxButton.OK, MessageBoxImage.Error);
                        Current.Shutdown(-1);
                        return;
                    }
                }

                ObjectContainer.Get<EditorViewModel>().LoadFile(fileToLoadAndRun);
                ObjectContainer.Get<InterpreterViewModel>().RunButtonClick();
            }
        }
    }
}
