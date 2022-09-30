using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ComponentCheckApp.Views.ReferenceEditor
{
    /// <summary>
    /// Interaction logic for ReferenceEditorView.xaml
    /// </summary>
    public partial class ReferenceEditorView : Window
    {
        public ReferenceEditorViewModel vm => (ReferenceEditorViewModel)DataContext;

        public ReferenceEditorView(WriteableBitmap referenceImage)
        {
            InitializeComponent();
            DataContext = new ReferenceEditorViewModel(referenceImage);
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            vm.MouseMove(sender, e);
        }
    }
}
