using Common;
using System.Windows;


namespace VisualBlocks.Settings
{
    internal class SettingsDC : DCBase
    {
        public void ShowWindow(Window mainWindow)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.DataContext = this;
            settingsWindow.Owner = mainWindow;
            settingsWindow.ShowDialog();
        }
    }
}
