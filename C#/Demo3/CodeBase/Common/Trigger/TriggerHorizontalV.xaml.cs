using System.Windows;
using System.Windows.Controls;


namespace Common.Trigger
{
    /// <summary>
    /// Interaction logic for TriggerView.xaml
    /// </summary>
    public partial class TriggerHorizontalV : UserControl
    {
        public TriggerHorizontalV()
        {
            InitializeComponent();
        }

        private void SingleStartButtonClick(object sender, RoutedEventArgs e) 
            => DataContext.CastTo<TriggerDC>().SingleStartButtonClick();
    }
}
