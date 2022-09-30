using Common;
using System.Windows;
using System.Windows.Controls;


namespace UCVisionResultExplorerApp
{
    /// <summary>
    /// Interaction logic for FilterV.xaml
    /// </summary>
    public partial class FilterV : UserControl
    {
        public FilterV()
        {
            InitializeComponent();
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<ResultExplorerDC>()?.ApplyFilter();
    }
}
