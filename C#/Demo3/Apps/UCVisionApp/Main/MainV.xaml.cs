using Common;
using System.Windows;
using System.Windows.Controls;


namespace UCVisionApp.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainV : Window
    {
        public MainV()
        {
            InitializeComponent();
        }

        private void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
            => DataContext.CastTo<MainDC>().TreeViewSelectedItemChanged(sender, e);

        private void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            TreeView treeView = sender as TreeView;

            if(treeView.IsNotNull())
                DataContext.CastTo<MainDC>().TreeView_Loaded(treeView, e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) { }
        //=> WindowState = WindowState.Maximized;
    }
}
