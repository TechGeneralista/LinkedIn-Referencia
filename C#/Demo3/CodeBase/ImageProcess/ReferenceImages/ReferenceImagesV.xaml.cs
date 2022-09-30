using Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ImageProcess.ReferenceImages
{
    public partial class ReferenceImagesV : UserControl
    {
        public ReferenceImagesV()
        {
            InitializeComponent();
        }

        private void RefreshReferenceImagesButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<ReferenceImagesDC>().Refresh();

        private void ShowColorReferenceImageButtonClick(object sender, MouseButtonEventArgs e)
         => DataContext.CastTo<ReferenceImagesDC>().ShowColorImage();

        private void ShowMonochromeReferenceImageButtonClick(object sender, MouseButtonEventArgs e)
         => DataContext.CastTo<ReferenceImagesDC>().ShowMonochromeImage();
    }
}
