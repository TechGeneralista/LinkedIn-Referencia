using System.Windows;
using System.Windows.Controls;


namespace ImageProcess.ContourFinder.UserContourPath
{
    /// <summary>
    /// Interaction logic for UserContourPathV.xaml
    /// </summary>
    public partial class UserContourPathV : UserControl
    {
        UserContourPathDC dc => (UserContourPathDC)DataContext;

        public UserContourPathV()
        {
            InitializeComponent();
        }

        private void RemoveAllButtonClick(object sender, RoutedEventArgs e) => dc.RemoveAll();
        private void ExchangeButtonClick(object sender, RoutedEventArgs e) => ((sender as Button)?.DataContext as UserLineDC)?.Exchange();
        private void RemoveButtonClick(object sender, RoutedEventArgs e) => ((sender as Button)?.DataContext as UserLineDC)?.SelfRemove();
    }
}
