using System.Windows.Controls;


namespace UniCamApp.Inspection
{
    /// <summary>
    /// Interaction logic for AreaV.xaml
    /// </summary>
    public partial class InspectionV : UserControl
    {
        public InspectionV()
        {
            InitializeComponent();
        }

        private void BackButtonClick(object sender, System.Windows.RoutedEventArgs e) => ((InspectionDC)DataContext).Back();
    }
}
