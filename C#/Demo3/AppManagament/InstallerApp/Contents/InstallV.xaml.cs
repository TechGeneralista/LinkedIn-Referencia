using Common;
using System.Windows;
using System.Windows.Controls;


namespace InstallerApp.Contents
{
    /// <summary>
    /// Interaction logic for InstallV.xaml
    /// </summary>
    public partial class InstallV : UserControl
    {
        public InstallV()
        {
            InitializeComponent();
        }

        private void FinishedButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<InstallDC>()?.FinishedButtonClick();
    }
}
