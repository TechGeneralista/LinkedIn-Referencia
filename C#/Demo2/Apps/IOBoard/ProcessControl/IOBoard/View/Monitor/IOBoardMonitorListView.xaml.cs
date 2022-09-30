using Common.Tool;
using System.Windows.Controls;


namespace IOBoard.View.Monitor
{
    /// <summary>
    /// Interaction logic for IOBoardMonitorListView.xaml
    /// </summary>
    public partial class IOBoardMonitorListView : UserControl
    {
        public IOBoardMonitorListView()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<IOBoardClient>();
        }
    }
}
