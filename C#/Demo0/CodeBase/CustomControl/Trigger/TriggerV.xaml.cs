using System.Windows;
using System.Windows.Controls;


namespace CustomControl.Trigger
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
        }

        private void SingleStartButtonClick(object sender, RoutedEventArgs e) => dc.CycleAsync();
    }
}
