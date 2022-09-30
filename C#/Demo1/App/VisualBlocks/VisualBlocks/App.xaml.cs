using Common;
using Compute;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Windows;
using VisualBlocks.Main;


namespace VisualBlocks
{
    public partial class App : Application
    {
        readonly string version = "3.0.5735";
        string filePathToOpen;
        MainDC mainDC;
        readonly string pipeServerName = $"{nameof(VisualBlocks)}PipeServer";


        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length != 0)
                filePathToOpen = e.Args[0];

            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length == 1)
                base.OnStartup(e);
            else
            {
                if (filePathToOpen != null)
                {
                    using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeServerName, PipeDirection.Out, PipeOptions.None))
                    {
                        pipeClient.Connect(1000);

                        if (pipeClient.IsConnected)
                        {
                            using (StreamWriter sw = new StreamWriter(pipeClient) { AutoFlush = true })
                            {
                                sw.WriteLine(filePathToOpen);
                                pipeClient.WaitForPipeDrain();
                            }

                            pipeClient.Close();
                        }
                    }
                }

                Shutdown(0);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Task.Run(()=> PipeServer());
            StartApplication();
        }

        private void PipeServer()
        {
            while (true)
            {
                string fileToOpen = null;

                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeServerName, PipeDirection.In))
                {
                    pipeServer.WaitForConnection();

                    using (StreamReader sr = new StreamReader(pipeServer))
                    {
                        fileToOpen = sr.ReadLine();
                    }
                }

                if(fileToOpen != null)
                {
                    Utils.InvokeIfNecessary(() => mainDC.ProjectDC.Open(fileToOpen));
                }
            }
        }

        private void StartApplication()
        {
            ComputeAccelerator computeAccelerator = new ComputeAccelerator(ComputeDeviceTypePriority.GPUCPU);

            MainV mainV = new MainV();
            mainV.DataContext = mainDC = new MainDC(version, computeAccelerator, mainV);
            mainV.Loaded += MainV_Loaded;
            MainWindow = mainV;
            MainWindow.Show();
        }

        private void MainV_Loaded(object sender, RoutedEventArgs e)
        {
            if (filePathToOpen != null)
                mainDC.ProjectDC.Open(filePathToOpen);
        }
    }
}
