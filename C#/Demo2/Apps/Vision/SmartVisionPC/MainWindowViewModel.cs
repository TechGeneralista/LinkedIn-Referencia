using SmartVisionClientApp.CameraSelect;
using SmartVisionClientApp.Common;
using SmartVisionClientApp.ImageOptimization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace SmartVisionClientApp
{
    internal class MainWindowViewModel
    {
        public ISettableObservableProperty<bool> ImageOptimizationButtonIsEnable { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<UserControl> CurrentContent { get; } = new ObservableProperty<UserControl>();
        public ISettableObservableProperty<BitmapSource> CurrentImage { get; } = new ObservableProperty<BitmapSource>();


        public MainWindowViewModel()
        {
            CameraButtonClick();
            CurrentImage.Value = Utils.GetBlackImage();
        }

        internal void CameraButtonClick() => CurrentContent.Value = ObjectContainer.Get<CameraSelectView>();
        internal void ImageOptimizationButtonClick() => CurrentContent.Value = new ImageOptimizationView() { DataContext = ObjectContainer.Get<Camera>() };
    }
}