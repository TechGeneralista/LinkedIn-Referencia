using System.Windows;
using System.Windows.Controls;


namespace VisualBlocks.Module.Trigger
{
    public partial class TriggerButtonV : UserControl
    {
        public TriggerButtonV()
        {
            InitializeComponent();
        }

        private void TriggerButtonClick(object sender, RoutedEventArgs e)
            => ((TriggerButtonDC)DataContext).Trigger();
    }
}
