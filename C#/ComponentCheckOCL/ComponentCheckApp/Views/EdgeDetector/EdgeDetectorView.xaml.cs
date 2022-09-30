using System.Windows.Controls;


namespace ComponentCheckApp.Views.EdgeDetector
{
    /// <summary>
    /// Interaction logic for CheckView.xaml
    /// </summary>
    public partial class EdgeDetectorView : UserControl
    {
        EdgeDetectorViewModel model;


        public EdgeDetectorView()
        {
            InitializeComponent();
            DataContext = model = new EdgeDetectorViewModel();
        }
    }
}
