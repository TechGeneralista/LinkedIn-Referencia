using Common;
using System.Windows;
using System.Windows.Controls;


namespace UniCamApp.Trigger
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

        private void SingleStartButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<TriggerViewModel>().SingleStartButtonClick();
    }
}
