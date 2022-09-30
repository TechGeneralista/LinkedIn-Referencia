using System.Windows.Controls;

namespace ComponentCheckApp.Views.Resize
{
    /// <summary>
    /// Interaction logic for ResizeView.xaml
    /// </summary>
    public partial class ResizeView : UserControl
    {
        public ResizeView()
        {
            InitializeComponent();
            DataContext = new ResizeViewModel();
        }
    }
}
