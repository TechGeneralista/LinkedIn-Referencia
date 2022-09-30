using Common.Tool;
using Editor;
using System.ComponentModel;
using System.Windows;


namespace ProcessControlApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<MainViewModel>();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e) => e.Cancel = ObjectContainer.Get<EditorViewModel>().Closing();
    }
}
