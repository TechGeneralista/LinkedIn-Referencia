using SmartVisionClientApp.Common;
using System.Windows.Controls;


namespace SmartVisionClientApp.Trigger
{
    /// <summary>
    /// Interaction logic for TriggerView.xaml
    /// </summary>
    public partial class TriggerView : UserControl
    {
        public TriggerView()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<TriggerViewModel>();
        }

        private void SingleStartButtonClick(object sender, System.Windows.RoutedEventArgs e) => ObjectContainer.Get<TriggerViewModel>().SingleStartButtonClick();
    }
}
