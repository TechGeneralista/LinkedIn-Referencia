using Common.Tool;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Editor
{
    /// <summary>
    /// Interaction logic for EditorView.xaml
    /// </summary>
    public partial class EditorView : UserControl
    {
        readonly string tab = "    ";

        public EditorView()
        {
            InitializeComponent();

            EditorViewModel editorViewModel = ObjectContainer.Get<EditorViewModel>();
            editorViewModel.View = this;
            DataContext = editorViewModel;
        }

        private void NewButtonClick(object sender, RoutedEventArgs e) => ((EditorViewModel)DataContext).NewButtonClick();
        private void OpenButtonClick(object sender, RoutedEventArgs e) => ((EditorViewModel)DataContext).OpenButtonClick();
        private void SaveButtonClick(object sender, RoutedEventArgs e) => ((EditorViewModel)DataContext).SaveButtonClick();

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.Key == Key.Tab)
            {
                int caretPosition = textBox.CaretIndex;
                textBox.Text = textBox.Text.Insert(caretPosition, tab);
                textBox.CaretIndex = caretPosition + tab.Length;
                e.Handled = true;
            }
        }
    }
}
