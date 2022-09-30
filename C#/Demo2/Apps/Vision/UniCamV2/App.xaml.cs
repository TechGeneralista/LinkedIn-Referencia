using Common;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using UniCamV2.Content.Model;
using UniCamV2.Content.View;
using UniCamV2.Tools;
using UniCamV2.Trigger;
using UVC;
using UVC.Internals;


namespace UniCamV2
{
    public partial class App : Application
    {
        static readonly string[] nameOfDLLFiles = { "Common.dll", "ImageProcess.dll", "OpenCLWrapper.dll", "UVC.dll" };

        OpenCLAccelerator openCLAccelerator;
        ImagePreparator imagePreparator;

        ImageSourceV imageSourceV;
        DevicePropertiesV devicePropertiesV;
        TaskListV taskListV;

        TriggerDC triggerDC;
        TriggerAutoStarter triggerAutoStarter;
        ImageSourceDC imageSourceDC;
        MainDC mainDC;
        TaskListDC taskListDC;

        MainV mainV;


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (CheckDLLFiles())
            {
                Shutdown(-1);
                return;
            }

            CreateObjectsInstances();
            SetInstancesDataContext();
            SetInstancesEventHandlers();

            MainWindow = mainV;
            MainWindow.Show();
        }

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

        private void CreateObjectsInstances()
        {
            openCLAccelerator = new OpenCLAccelerator(DeviceTypePriority.GPUCPU);
            imagePreparator = new ImagePreparator(openCLAccelerator);

            imageSourceV = new ImageSourceV();
            devicePropertiesV = new DevicePropertiesV();
            taskListV = new TaskListV();

            triggerDC = new TriggerDC();
            ObjectContainer.Set(triggerDC);
            triggerAutoStarter = new TriggerAutoStarter(triggerDC);

            mainV = new MainV();

            mainDC = new MainDC(new UserControl[] { imageSourceV, devicePropertiesV, taskListV }, triggerAutoStarter);
            imageSourceDC = new ImageSourceDC();
            taskListDC = new TaskListDC(imagePreparator, mainDC, openCLAccelerator);
        }

        private void SetInstancesDataContext()
        {
            imageSourceV.DataContext = imageSourceDC;
            taskListV.DataContext = taskListDC;
            mainV.DataContext = mainDC;
        }

        private void SetInstancesEventHandlers()
        {
            mainV.Closing += MainV_Closing;
            imageSourceDC.Connected += triggerDC.Enable;
            imageSourceDC.Connected += mainDC.Enable;
            imageSourceDC.Connected += (s, d) => devicePropertiesV.DataContext = d.Properties;
            imageSourceDC.Connected += CaptureAndShowColorImage;
            imageSourceDC.Disconnected += mainDC.Disable;
            imageSourceDC.Disconnected += triggerDC.Disable;
            imageSourceDC.Disconnected += (s, d) => devicePropertiesV.DataContext = null;
            imageSourceDC.Disconnected += (s, d) => imagePreparator.ImageSource = null;
            triggerDC.CaptureNewImage += (s) => imagePreparator.Capture();
            triggerDC.Shoot += Cycle;
            taskListDC.SetMainContent += (s, c) => mainDC.CurrentContent.Value = c;
            mainDC.MainDisplayMouseDown += taskListDC.MainDisplayMouseDown;
            mainDC.MainDisplayMouseMove += taskListDC.MainDisplayMouseMove;
            mainDC.MainDisplayMouseUp += taskListDC.MainDisplayMouseUp;
        }

        private void CaptureAndShowColorImage(object sender, IImageSource imageSource)
        {
            imagePreparator.ImageSource = imageSource;
            imagePreparator.Capture();
            mainDC.UserScreen.Display.Value = imagePreparator.ColorImageBuffer.Upload();
        }

        private void MainV_Closing(object sender, CancelEventArgs e)
        {
            triggerDC.Shutdown();
            imageSourceDC.Shutdown();
        }

        private void Cycle(object sender)
        {
            Type currentContentType = mainDC.CurrentContent.Value.GetType();

            if (currentContentType == typeof(ImageSourceV) || currentContentType == typeof(DevicePropertiesV))
                mainDC.UserScreen.Display.Value = imagePreparator.ColorImageBuffer.Upload();
            else
                taskListDC.NewImage();
        }
    }
}
