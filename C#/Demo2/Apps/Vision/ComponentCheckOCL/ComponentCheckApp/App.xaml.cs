using CommonLib.Interfaces;
using ComponentCheckApp.Components;
using ComponentCheckApp.Views.DeviceSelect;
using D4I4OLib;
using ImageProcessLib.OpenCL;
using ImageProcessLib.OpenCL.Compute;
using ImageProcessLib.Views.Wait;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace ComponentCheckApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        DllFiles dllFiles;
        OpenCLDevice openCLDevice;
        D4I4O d4I4O;


        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            //Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            dllFiles = new DllFiles();
            openCLDevice = new OpenCLDevice();
            d4I4O = new D4I4O();
            d4I4O.GUID = "F3EC712543224F25A567DF549165FD17";
            d4I4O.Out0 = true;
            d4I4O.Out1 = true;

            Current.Resources[ResourceKeys.D4I4OKey] = d4I4O;
            Current.Resources[ResourceKeys.OpenCLDeviceKey] = openCLDevice;

            bool deviceSelected = false;

            if (e.Args.Length == 0)
                deviceSelected = openCLDevice.Select(ComputeDeviceTypes.Gpu);
            else
            {
                if (e.Args[0] == "cpu")
                    deviceSelected = openCLDevice.Select(ComputeDeviceTypes.Cpu);
                else if (e.Args[0] == "gpu")
                    deviceSelected = openCLDevice.Select(ComputeDeviceTypes.Gpu);
                else
                    deviceSelected = openCLDevice.Select(ComputeDeviceTypes.Gpu);
            }

            if (dllFiles.FileExistChecker.AreExist() && deviceSelected)
            {
                WaitView2 waitView = new WaitView2("Starting");
                waitView.Show();

                await Task.Run(() =>
                {
                    Current.Dispatcher.Invoke(() => MainWindow = new MainWindow() );
                });

                waitView.Close();

                MainWindow.Title += " - " + openCLDevice.ComputeDevice.Name;
                MainWindow.Closing += MainWindow_Closing;
                MainWindow.Show();
            }
            else
                Shutdown(-1);

            d4I4O.Out0 = false;
            d4I4O.Out1 = true;
            d4I4O.Out2 = false;
            d4I4O.Out3 = true;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            DeviceSelectViewModel deviceSelectVM = (DeviceSelectViewModel)Current.Resources[ResourceKeys.DeviceSelectViewModelKey];
            IVideoCaptureDevice captureDevice = deviceSelectVM.DeviceSelectModel.ConnectedUVCDevice;

            if (captureDevice != null)
                captureDevice.Disconnect();

            d4I4O.Out0 = false;
            d4I4O.Out1 = false;
            d4I4O.Out2 = false;
            d4I4O.Out3 = false;
            Thread.Sleep(250);
        }
    }
}
