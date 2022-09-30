using System.Windows;
using System.Windows.Controls;


namespace VisualBlocks.Module.Trigger
{
    public partial class TriggerInitializeV : UserControl
    {
        public TriggerInitializeV()
        {
            InitializeComponent();
        }

        private void TriggerButtonClick(object sender, RoutedEventArgs e)
            => ((TriggerInitializeDC)DataContext).Trigger();
    }
}
