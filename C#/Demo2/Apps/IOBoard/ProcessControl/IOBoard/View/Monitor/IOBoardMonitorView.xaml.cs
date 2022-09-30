using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace IOBoard.View.Monitor
{
    /// <summary>
    /// Interaction logic for IOBoardMonitorView.xaml
    /// </summary>
    public partial class IOBoardMonitorView : UserControl
    {
        public IOBoardMonitorView()
        {
            InitializeComponent();
        }

        private void Toggle(object sender, MouseButtonEventArgs e) => ((IOBoardDigitalOutput)((FrameworkElement)sender).DataContext).Toggle();
    }
}
