using System.Windows;


namespace ImageProcessLib.Views.Wait
{
    /// <summary>
    /// Interaction logic for WaitView.xaml
    /// </summary>
    public partial class WaitView : Window
    {
        public WaitView(WaitViewModel wvm)
        {
            InitializeComponent();
            DataContext = wvm;
        }
    }
}
