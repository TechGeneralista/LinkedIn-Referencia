using Common;
using System.Windows.Controls;


namespace UniCamApp.TaskList
{
    /// <summary>
    /// Interaction logic for TasksView.xaml
    /// </summary>
    public partial class TaskListView : UserControl
    {
        public TaskListView()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<TaskListViewModel>();
        }


        private void ClearSelectionButtonClick(object sender, System.Windows.RoutedEventArgs e) => ((TaskListViewModel) DataContext).ClearSelectionButtonClick();
        private void DeleteButtonClick(object sender, System.Windows.RoutedEventArgs e) => ((TaskListViewModel)DataContext).DeleteButtonClick();
        private void TaskListMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => ((TaskListViewModel)DataContext).TaskListMouseDoubleClick();
        private void AddColorAreaV1ButtonClick(object sender, System.Windows.RoutedEventArgs e) => ((TaskListViewModel)DataContext).AddColorAreaV1ButtonClick();
    }
}
