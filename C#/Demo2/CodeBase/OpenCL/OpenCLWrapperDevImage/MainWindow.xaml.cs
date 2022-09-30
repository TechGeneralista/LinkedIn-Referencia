using System.Windows;


namespace OpenCLWrapperDevImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel vm => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

    }
}
