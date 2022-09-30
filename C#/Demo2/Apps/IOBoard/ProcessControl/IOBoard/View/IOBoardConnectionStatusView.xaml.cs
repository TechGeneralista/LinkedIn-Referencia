using Common.Tool;
using System.Windows.Controls;


namespace IOBoard.View
{
    /// <summary>
    /// Interaction logic for IOBoardConnectionStatusView.xaml
    /// </summary>
    public partial class IOBoardConnectionStatusView : UserControl
    {
        public IOBoardConnectionStatusView()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<IOBoardClient>();
        }
    }
}
