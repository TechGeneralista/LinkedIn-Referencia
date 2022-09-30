using Common;
using System.Windows;
using System.Windows.Controls;


namespace UniCamV2.Trigger
{
    /// <summary>
    /// Interaction logic for TriggerView.xaml
    /// </summary>
    public partial class TriggerV : UserControl
    {
        TriggerDC dc => (TriggerDC)DataContext;

        public TriggerV()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<TriggerDC>();
        }

        private void SingleStartButtonClick(object sender, RoutedEventArgs e) => dc.StartCycle();
    }
}
