using AppLog;
using Common.Interface;
using Common.NotifyProperty;
using Common.Settings;
using CustomControl.Id;
using CustomControl.ImageViewControl;
using CustomControl.Trigger;
using ImageProcess.OpticalDistortionCorrection;
using ImageProcess.Source;
using ImageSourceDevice;
using Language;
using LogicalEvaluator;
using OpenCLWrapper;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UniCamApp.Inspections;


namespace UniCamApp.Instance
{
    public class InstanceDC : ICanProcess, ICanShowResultImage, ICanStart, ICanStop, IHasImageSourceDevice, IHasTriggerDC, ICanHandleImageViewControlMouseEvents, IHasId, IHasResult, ICanSaveLoadSettings, ICanResetContent
    {
        public IdDC IdDC => LogicalEvaluatorDC.IdDC;
        public INonSettableObservableProperty<bool?> Result => LogicalEvaluatorDC.Result;

        public LanguageDC LanguageDC { get; }
        public IImageSourceDevice ImageSourceDevice { get; }
        public OpticalDistortionCorrectionDC OpticalDistortionCorrectionDC { get; }
        public ImageProcessSourceDC ImageProcessSourceDC { get; }
        public TriggerDC TriggerDC { get; }
        public ISettableObservableProperty<ImageSource> ResultImage { get; } = new ObservableProperty<ImageSource>();
        public ISettableObservableProperty<object> CurrentContent { get; } = new ObservableProperty<object>();
        public ISettableObservableProperty<int> SelectedTabItemIndex { get; } = new ObservableProperty<int>();
        public InspectionsDC InspectionsDC { get; }
        public LogicalEvaluatorDC LogicalEvaluatorDC { get; }


        readonly ImageSourceDeviceOutputDownloader imageSourceDeviceOutputDownloader;


        public InstanceDC(LanguageDC languageDC, IImageSourceDevice imageSourceDevice, OpenCLAccelerator openCLAccelerator, ILog log)
        {
            LanguageDC = languageDC;
            ImageSourceDevice = imageSourceDevice;

            imageSourceDeviceOutputDownloader = new ImageSourceDeviceOutputDownloader(openCLAccelerator);
            OpticalDistortionCorrectionDC = new OpticalDistortionCorrectionDC(languageDC, openCLAccelerator);
            ImageProcessSourceDC = new ImageProcessSourceDC(openCLAccelerator, CapturePrepare);

            InspectionsDC = new InspectionsDC(languageDC, openCLAccelerator, ImageProcessSourceDC, ResultImage, CurrentContent, log, this);
            LogicalEvaluatorDC = new LogicalEvaluatorDC(languageDC, log, InspectionsDC.Inspections);

            TriggerDC = new TriggerDC(languageDC, LogicalEvaluatorDC.IdDC.Id);
            TriggerDC.BeforeShoot += TriggerDC_BeforeShoot;
            TriggerDC.Shoot += TriggerDC_Shoot;

            ResetContent();
        }

        public void ResetContent()
        {
            CurrentContent.CurrentValue = this;
            SelectedTabItemIndex.CurrentValue = 0;
            InspectionsDC.ResetContent();
        }

        private void TriggerDC_BeforeShoot() => CapturePrepare();

        private void TriggerDC_Shoot()
        {
            Process();
            ShowResultImage();
        }

        public void Start()
        {
            ImageSourceDevice.Start();
            Thread.Sleep(2000);
            CapturePrepare();
        }

        public void CapturePrepare()
        {
            ImageSourceDevice.Capture();
            imageSourceDeviceOutputDownloader.Download(ImageSourceDevice.Output);
            OpticalDistortionCorrectionDC.Correct(imageSourceDeviceOutputDownloader.OutputImageBuffer);
            ImageProcessSourceDC.Prepare(OpticalDistortionCorrectionDC.OutputImageBuffer);
        }

        public void Stop() => ImageSourceDevice.Stop();

        public void ImageViewControlMouseDown(Point downPosition, MouseButtonEventArgs e) => InspectionsDC.ImageViewControlMouseDown(downPosition, e);
        public void ImageViewControlMouseMove(Vector moveVector, Point movePosition, MouseEventArgs e) => InspectionsDC.ImageViewControlMouseMove(moveVector, movePosition, e);
        public void ImageViewControlMouseUp(Point upPosition, MouseButtonEventArgs e) => InspectionsDC.ImageViewControlMouseUp(upPosition, e);

        public void Process()
        {
            InspectionsDC.Process();
            LogicalEvaluatorDC.Evulate();
        }
        public void ShowResultImage()
        {
            switch (SelectedTabItemIndex.CurrentValue)
            {
                case 0:
                    InspectionsDC.ShowResultImage();
                    break;

                case 1:
                    ResultImage.CurrentValue = ImageProcessSourceDC.ColorImage.CurrentValue;
                    break;
            }
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(InstanceDC));
            TriggerDC.SaveSettings(settingsCollection);
            (ImageSourceDevice as ICanSaveLoadSettings)?.SaveSettings(settingsCollection);
            OpticalDistortionCorrectionDC.SaveSettings(settingsCollection);
            InspectionsDC.SaveSettings(settingsCollection);
            LogicalEvaluatorDC.SaveSettings(settingsCollection);
            settingsCollection.KeyCreator.RemoveLast();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(InstanceDC));
            TriggerDC.LoadSettings(settingsCollection);
            (ImageSourceDevice as ICanSaveLoadSettings)?.LoadSettings(settingsCollection);
            OpticalDistortionCorrectionDC.LoadSettings(settingsCollection);
            InspectionsDC.LoadSettings(settingsCollection);
            LogicalEvaluatorDC.LoadSettings(settingsCollection);
            settingsCollection.KeyCreator.RemoveLast();
        }
    }
}
