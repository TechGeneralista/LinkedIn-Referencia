using Common;
using Common.Communication.SimpleTCP.Server.MultiClient;
using Common.Interfaces;
using Common.Language;
using Common.License;
using Common.Log;
using Common.PopupWindow;
using Common.Settings;
using OpenCLWrapper;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using UCVisionApp.Main;
using UCVisionApp.Remote;


namespace UCVisionApp
{
    public partial class App : Application, ICanRemote
    {
        readonly string applicatonName = "UCVision";
        //readonly string hostName = "192.168.0.105";
        readonly string hostName = "185.75.194.85";
        readonly int serverPort = 8610;
        ISettingsCollection settingsCollection;
        LanguageDC languageDC;
        MainDC mainDC;
        LicenseDC licenseDC;


        protected override void OnStartup(StartupEventArgs e)
        {
            int appInstanceIndex = Process.GetProcessesByName(nameof(UCVisionApp)).Length;

            if (appInstanceIndex != 1)
            {
                MessageBox.Show("The program cannot be started because an instance is already running on the computer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(-1);
                return;
            }

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            DLLFiles dllFiles = new DLLFiles();
            dllFiles.AddFileName(nameof(Common));
            dllFiles.AddFileName(nameof(ImageCaptureDevice));
            dllFiles.AddFileName(nameof(ImageProcess));
            dllFiles.AddFileName(nameof(LogicalEvaluator));
            dllFiles.AddFileName(nameof(OpenCLWrapper));

            if (!dllFiles.CheckExistence())
            {
                Shutdown(-1);
                return;
            }

            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppInfo appInfo = new AppInfo(Constants.SmartSolutionsDebrecen, applicatonName, Assembly.GetExecutingAssembly().GetName().Version);
            settingsCollection = new SettingsCollection(new FileSettingsStore(appInfo.ConfigFilePath), true);

            languageDC = new LanguageDC(settingsCollection);

            LogDC logDC = new LogDC(languageDC, 100);
            OpenCLAccelerator openCLAccelerator = new OpenCLAccelerator(languageDC, logDC);

            if (!openCLAccelerator.ScanDevice(DeviceTypePriority.GPUCPU))
            {
                Shutdown(-1);
                return;
            }

            MainWindow = new MainV();
            MainWindow.Closing += MainWindow_Closing;

            CommunicationParser communicationParser = new CommunicationParser(this);
            TCPServerMultiClientDC tcpServerMultiClientDC = new TCPServerMultiClientDC(languageDC, settingsCollection, logDC, communicationParser);

            ApplicationClient applicationClient = new ApplicationClient(languageDC, appInfo, MainWindow, hostName, serverPort, Utils.ConvertMegaBytesToBytes(1), Utils.ConvertMegaBytesToBytes(10), Utils.ConvertSecondsToMilliSeconds(30), Constants.DataEnd, Constants.DivisionSign.ToString());
            licenseDC = new LicenseDC(languageDC, appInfo, applicationClient, MainWindow);
            licenseDC.Exit += LicenseDC_Exit;
            licenseDC.CheckAsync(Utils.ConvertMinutesToMilliseconds(60));

            mainDC = new MainDC(languageDC, logDC, openCLAccelerator, new PopupWindowDC(languageDC, MainWindow), tcpServerMultiClientDC, settingsCollection, licenseDC.TitleText, appInfo);
            MainWindow.DataContext = mainDC;
            MainWindow.Show();

            logDC.NewMessage(LogTypes.Successful, languageDC.ApplicationStarted);
            mainDC.Children.ForEach(x => x.CastTo<ICanStartAutoLoad>()?.StartAutoLoad());
        }

        public string Remote(string command, string[] ids)
        {
            string response = null;
            response = mainDC.CastTo<ICanRemote>().Remote(command, ids);
            return response;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            settingsCollection.Write();

            if (settingsCollection.SettingsStore.IsErrorOccured)
                MessageBox.Show(languageDC.SaveSettings.Value, languageDC.ErrorColon.Value, MessageBoxButton.OK, MessageBoxImage.Error);

            licenseDC.ApplicationExit();
        }

        private void LicenseDC_Exit()
            => Utils.InvokeIfNecessary(() => Shutdown(-1));
    }
}
