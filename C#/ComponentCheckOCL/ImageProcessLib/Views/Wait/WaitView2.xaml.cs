using System.Windows;


namespace ImageProcessLib.Views.Wait
{
    /// <summary>
    /// Interaction logic for WaitView.xaml
    /// </summary>
    public partial class WaitView2 : Window
    {
        public string Information { get; private set; }

        public WaitView2(string information)
        {
            InitializeComponent();
            Information = information;
            DataContext = this;
        }
    }
}
