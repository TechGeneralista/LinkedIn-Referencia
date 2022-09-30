using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace VisualBlocks.Settings
{
    public partial class SettingsWindow : Window
    {
        readonly DoubleAnimation fadeIn, fadeOut;


        public SettingsWindow()
        {
            InitializeComponent();
            Loaded += SettingsWindow_Loaded;
            Closing += SettingsWindow_Closing;

            fadeIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(250));
            fadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(250));
            fadeOut.Completed += FadeOut_Completed;
            Opacity = 0;
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
            => BeginAnimation(OpacityProperty, fadeIn);

        private void SettingsWindow_Closing(object sender, CancelEventArgs e)
        {
            if(Opacity == 1d)
            {
                e.Cancel = true;
                BeginAnimation(OpacityProperty, fadeOut);
            }
        }

        private void FadeOut_Completed(object sender, EventArgs e)
            => Close();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void CloseButtonClick(object sender, RoutedEventArgs e)
            => Close();
    }
}
