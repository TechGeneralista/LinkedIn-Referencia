using Common;
using System.Windows;
using System.Windows.Controls;


namespace uVisionV2.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainV : Window
    {
        MainDC mainDC => (MainDC)DataContext;


        public MainV()
        {
            InitializeComponent();
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
            => mainDC.SettingsButtonClick();

        private void AddButtonClick(object sender, RoutedEventArgs e)
            => Utils.OpenContextMenu(sender, e);

        private void AddTriggerButtonClick(object sender, RoutedEventArgs e)
            => mainDC.AddTriggerButtonClick();

        private void TopButtonContextMenuLoaded(object sender, RoutedEventArgs e)
            => ((ContextMenu)sender).DataContext = mainDC;
    }
}
