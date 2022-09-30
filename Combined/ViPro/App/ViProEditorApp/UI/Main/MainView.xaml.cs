using Microsoft.Win32;
using System;
using System.Windows;
using ViProEditorApp.UI.Project;

namespace ViProEditorApp.UI.Main
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            SizeChanged += EditorView_SizeChanged;
        }

        private void EditorView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                treeViewColumn.MaxWidth = e.NewSize.Width / 2;
                e.Handled = true;
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ((MainViewModel)DataContext).TreeViewSelectedItem = (NotifyBase)e.NewValue;
            e.Handled = true;
        }

        private void CloseProjectClicked(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).Close();
            e.Handled = true;
        }

        private void SaveProjectClicked(object sender, RoutedEventArgs e)
        {
            var mainViewModel = (MainViewModel)DataContext;

            if (mainViewModel.FilePath == null)
                SaveAsProjectClicked(sender, e);
            else
                mainViewModel.Save(mainViewModel.FilePath);

            e.Handled = true;
        }

        private void SaveAsProjectClicked(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "ps4 files (*.ps4)|*.ps4";

            if (saveFileDialog.ShowDialog() is bool b && b)
            {
                var mainViewModel = (MainViewModel)DataContext;

                try
                {
                    mainViewModel.Save(saveFileDialog.FileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            e.Handled = true;
        }

        private void AddNewDeviceToProjectClicked(object sender, RoutedEventArgs e)
            => ((ProjectViewModel)((FrameworkElement)sender).DataContext).AddNewDevice();
    }
}
