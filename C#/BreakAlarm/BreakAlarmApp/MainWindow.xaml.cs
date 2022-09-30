using System.Windows;
using System.Windows.Controls;

namespace BreakAlarmApp
{
    public partial class MainWindow : Window
    {
        MainDC dc => (MainDC)DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TimedAlarmAddNewButtonClick(object sender, RoutedEventArgs e) => dc.TimedAlarms.AddNew();
        private void TimedAlarmBrowseSoundFileButtonClick(object sender, RoutedEventArgs e) => ((TimedAlarm)((Button)sender).DataContext).AlarmSoundPlayer.BrowseSoundFile();
        private void TimedAlarmRemoveButtonClick(object sender, RoutedEventArgs e) => ((TimedAlarm)((Button)sender).DataContext).Remove();
        private void WriteButtonClick(object sender, RoutedEventArgs e) => dc.Write();
        //private void ExernalAlarmAddNewButtonClick(object sender, RoutedEventArgs e) => dc.ExternalAlarms.AddNew();
        private void ExternalAlarmBrowseSoundFileButtonClick(object sender, RoutedEventArgs e) => ((ExternalAlarm)((Button)sender).DataContext).AlarmSoundPlayer.BrowseSoundFile();
        private void ExternalAlarmRemoveButtonClick(object sender, RoutedEventArgs e) => ((ExternalAlarm)((Button)sender).DataContext).Remove();

        private void TemponaryButtonClick(object sender, RoutedEventArgs e) => dc.TimedAlarms.EnableDisableTemponaries();
    }
}
