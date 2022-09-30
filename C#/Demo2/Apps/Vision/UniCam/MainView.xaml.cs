using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using UniCamApp.TaskList;
using Common;


namespace UniCamApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<MainViewModel>();
        }

        private void ShowImageSourceSelectContentButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<MainViewModel>().ShowImageSourceSelectContentButtonClick();
        private void ShowImageOptimizationContentButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<MainViewModel>().ShowImageOptimizationContentButtonClick();
        private void ShowTasksContentButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<MainViewModel>().ShowTasksContentButtonClick();
        private void ImageMouseMove(object sender, MouseEventArgs e) => ObjectContainer.Get<MainViewModel>().ImageMouseMove((Image)sender, e);
        private void ImageMouseDown(object sender, MouseButtonEventArgs e) => ObjectContainer.Get<MainViewModel>().ImageMouseDown((Image)sender, e);
        private void ImageMouseUp(object sender, MouseButtonEventArgs e) => e.MouseDevice.Capture(null);
        private void ImageDoubleClick(object sender, MouseButtonEventArgs e) => ObjectContainer.Get<TaskListViewModel>().TaskListMouseDoubleClick();
    }
}
