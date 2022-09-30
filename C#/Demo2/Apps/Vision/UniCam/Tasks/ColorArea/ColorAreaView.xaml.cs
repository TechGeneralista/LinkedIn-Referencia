using Common.MouseTool;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace UniCamApp.Tasks.ColorArea
{
    /// <summary>
    /// Interaction logic for ColorAreaView.xaml
    /// </summary>
    public partial class ColorAreaView : UserControl
    {
        public ColorAreaView() => InitializeComponent();

        private void ImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            ImageMousePosition.GetPosition((Image)sender, e);
            ((ColorAreaViewModel)DataContext).ImageMouseDown(ImageMousePosition.Position);
        }

        private void ClearRegisteredColorsList(object sender, RoutedEventArgs e) => ((ColorAreaViewModel)DataContext).ClearRegisteredColorsList();
    }
}
