using System.Windows.Controls;
using System.Windows.Input;


namespace ComponentCheckApp.Views.DigitalZoom
{
    /// <summary>
    /// Interaction logic for DigitalZoomView.xaml
    /// </summary>
    public partial class DigitalZoomView : UserControl
    {
        DigitalZoomViewModel model;

        public DigitalZoomView()
        {
            InitializeComponent();
            DataContext = model = new DigitalZoomViewModel();
        }

        private void OriginalImageMouseMove(object sender, MouseEventArgs e)
        {
            model.SelectionRectangle.MouseMove((Image)sender, e);
        }

        private void OriginalImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            model.SelectionRectangle.MouseDown((Image)sender, e);
        }

        private void OriginalImageMouseUp(object sender, MouseButtonEventArgs e)
        {
            model.SelectionRectangle.MouseUp((Image)sender, e);
        }
    }
}
