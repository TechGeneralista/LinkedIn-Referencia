using System.Windows;
using System.Windows.Controls;


namespace ImageCaptureDevice.UniversalVideoClass.CamVidProperties
{
    public partial class UVCPropertiesV : UserControl
    {
        public UVCPropertiesV()
        {
            InitializeComponent();
        }

        private void SetAllToDefaultButtonClick(object sender, RoutedEventArgs e)
            => ((IImageCaptureDeviceProperties)DataContext).ResetAllToDefault();
    }
}
