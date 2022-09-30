using Common;
using Common.NotifyProperty;
using OpenCLWrapper;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UniCamApp.ImageSourceSelector;
using UniCamApp.TaskList;
using UniCamApp.Trigger;
using UVC.Internals;


namespace UniCamApp
{
    public class MainViewModel
    {
        public string Title { get; }
        public ISettableObservableProperty<WriteableBitmap> ImageSource { get; } = new ObservableProperty<WriteableBitmap>();
        public ISettableObservableProperty<UserControl> CurrentContent { get; } = new ObservableProperty<UserControl>();
        public ISettableObservableProperty<bool> ShowImageOptimizationContentButtonIsEnable { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<bool> ShowTasksContentButtonIsEnable { get; } = new ObservableProperty<bool>();


        public MainViewModel()
        {
            Title = "UniCam V1.0 - Accelerator: " + ObjectContainer.Get<OpenCLAccelerator>().Context.Device.Name;
            ShowImageSourceSelectContentButtonClick();
            SetBlackImage();
        }

        public void SetBlackImage()
        {
            WriteableBitmap blackImage = new WriteableBitmap(4, 3, 96, 96, PixelFormats.Bgr24, null);
            blackImage.Freeze();
            ImageSource.Value = blackImage;
        }

        internal void ShowImageSourceSelectContentButtonClick() => CurrentContent.Value = ObjectContainer.Get<ImageSourceSelectorView>();

        internal void ShowImageOptimizationContentButtonClick()
        {
            DevicePropertiesV imageOptimizationView = ObjectContainer.Get<DevicePropertiesV>();
            imageOptimizationView.DataContext = ObjectContainer.Get<ImageSourceSelectorViewModel>().SelectedDevice.Value.Properties;
            CurrentContent.Value = imageOptimizationView;
        }

        internal void ShowTasksContentButtonClick()
        {
            CurrentContent.Value = ObjectContainer.Get<TaskListView>();
            ObjectContainer.Get<TaskListViewModel>().ClearSelection();
        }

        internal void ImageMouseMove(System.Windows.Controls.Image sender, MouseEventArgs e)
        {
            if (!IsCurrentContentImageSourceSelectorOrImageOptimization() && IsTriggerNotInternalMode())
                ObjectContainer.Get<TaskListViewModel>().ImageMouseMove(sender,e);
        }

        internal void ImageMouseDown(System.Windows.Controls.Image sender, MouseButtonEventArgs e)
        {
            if (!IsCurrentContentImageSourceSelectorOrImageOptimization() && IsTriggerNotInternalMode())
                ObjectContainer.Get<TaskListViewModel>().ImageMouseDown(sender, e);
        }

        public bool IsCurrentContentImageSourceSelectorOrImageOptimization()
        {
            if (CurrentContent.Value.IsNull())
                return false;

            Type currentShowType = CurrentContent.Value.GetType();
            return currentShowType == typeof(ImageSourceSelectorView) || currentShowType == typeof(DevicePropertiesV);
        }

        private bool IsTriggerNotInternalMode()
        {
            TriggerViewModel triggerViewModel = ObjectContainer.Get<TriggerViewModel>();
            return !triggerViewModel.IsInternalTrigger.Value;
        }
    }
}