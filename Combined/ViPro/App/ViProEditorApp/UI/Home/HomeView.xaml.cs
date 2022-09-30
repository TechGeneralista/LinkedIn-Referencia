using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using ViProEditorApp.UI.Main;

namespace ViProEditorApp.UI.Home
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void CreateNewProjectClicked(object sender, RoutedEventArgs e)
            => ((HomeViewModel)DataContext).CreateNewProject();

        private void OpenExistProjectClicked(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ps4 files (*.ps4)|*.ps4";

            if (openFileDialog.ShowDialog() is bool b && b)
            {
                var homeViewModel = (HomeViewModel)DataContext;

                try
                {
                    homeViewModel.OpenExistProject(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, ex.ToString(), "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            e.Handled = true;
        }
    }
}
