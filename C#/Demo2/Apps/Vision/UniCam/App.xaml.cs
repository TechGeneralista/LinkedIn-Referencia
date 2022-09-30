using Common;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using UniCamApp.ImageSourceSelector;
using UniCamApp.TaskList;
using UniCamApp.Trigger;
using UVC.Internals;

namespace UniCamApp
{
    public partial class App : Application
    {
        static readonly string[] nameOfDLLFiles = { "Common.dll", "ImageProcess.dll", "OpenCLWrapper.dll", "UVC.dll" };

        private bool CheckDLLFiles()
        {
            foreach (string s in nameOfDLLFiles)
            {
                if (!File.Exists(s))
                {
                    MessageBox.Show(string.Format("A program nem indítható el mert '{0}' fájl hiányzik!", s), "Hiba:", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true;
                }
            }

            return false;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool err = CheckDLLFiles();

            OpenCLAccelerator openCLAccelerator = new OpenCLAccelerator(DeviceTypePriority.GPUCPU);
            ObjectContainer.Set(openCLAccelerator);

            ObjectContainer.Set(new ImageSourceSelectorViewModel());
            ObjectContainer.Set(new ImageSourceSelectorView());

            ObjectContainer.Set(new DevicePropertiesV());

            ObjectContainer.Set(new TaskListViewModel());
            ObjectContainer.Set(new TaskListView());

            ObjectContainer.Set(new TriggerViewModel());
            ObjectContainer.Set(new MainViewModel());
            ObjectContainer.Set(new MainView());

            MainWindow = ObjectContainer.Get<MainView>();
            MainWindow.Closing += MainWindow_Closing;
            MainWindow.Show();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            ObjectContainer.Get<TriggerViewModel>().Shutdown();
            ObjectContainer.Get<ImageSourceSelectorViewModel>().Shutdown();
            ObjectContainer.Get<ImageSourceSelectorViewModel>().StopButtonClick();
        }
    }
}
