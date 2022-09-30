using System.Windows;


namespace VisualBlocks.Main
{
    public partial class MainV : Window
    {
        MainDC dc => (MainDC)DataContext;

        public MainV()
        {
            InitializeComponent();
        }
    }
}
