using System.Windows;
using System.Windows.Controls;


namespace Common.Controls.PanZoomCanvasView.ModuleContainer
{
    /// <summary>
    /// Interaction logic for ModuleContainerV.xaml
    /// </summary>
    public partial class ModuleContainerV : UserControl
    {
        public static readonly DependencyProperty ModuleContentProperty = DependencyProperty.Register(nameof(ModuleContent), typeof(object), typeof(ModuleContainerV));


        public object ModuleContent
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }


        ModuleContainerDC moduleContainerDC => (ModuleContainerDC)DataContext;


        public ModuleContainerV()
        {
            InitializeComponent();
            Loaded += (s, e) => ((ModuleContainerDC)DataContext).Initialize(this);
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
            => moduleContainerDC.DeleteButtonClick();
    }
}
