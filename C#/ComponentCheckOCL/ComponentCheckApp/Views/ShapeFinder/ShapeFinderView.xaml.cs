using System.Windows.Controls;

namespace ComponentCheckApp.Views.ShapeFinder
{
    /// <summary>
    /// Interaction logic for ShapeFinderView.xaml
    /// </summary>
    public partial class ShapeFinderView : UserControl
    {
        ShapeFinderViewModel vm => (ShapeFinderViewModel)DataContext;

        public ShapeFinderView()
        {
            InitializeComponent();
            DataContext = new ShapeFinderViewModel();
        }

        private void CreateButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.Create();
        }

        private void RemoveButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.RemoveSelected();
        }
    }
}
