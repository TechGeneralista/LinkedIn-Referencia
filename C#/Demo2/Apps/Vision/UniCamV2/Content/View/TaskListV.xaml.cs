using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniCamV2.Content.Model;


namespace UniCamV2.Content.View
{
    /// <summary>
    /// Interaction logic for TaskListView.xaml
    /// </summary>
    public partial class TaskListV : UserControl
    {
        TaskListDC dc => (TaskListDC)DataContext;

        public TaskListV() => InitializeComponent();

        private void AddNewButtonClick(object sender, RoutedEventArgs e) => dc.AddNewArea();
        private void RemoveSelectedButtonClick(object sender, RoutedEventArgs e) => dc.RemoveSelectedArea();
        private void RemoveAllButtonClick(object sender, RoutedEventArgs e) => dc.RemoveAllAreas();
        private void AreaDoubleClick(object sender, MouseButtonEventArgs e) => dc.ShowAreaProperties();
    }
}
