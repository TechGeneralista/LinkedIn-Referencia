using Common;
using System.Windows;
using System.Windows.Controls;


namespace UCVisionResultExplorerApp
{
    /// <summary>
    /// Interaction logic for ResultExplorerSourceV.xaml
    /// </summary>
    public partial class ResultExplorerSourceV : UserControl
    {
        public ResultExplorerSourceV()
        {
            InitializeComponent();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<ResultExplorerDC>().BrowseButtonClick();
    }
}
