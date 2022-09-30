using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


namespace CustomControl
{
    public class IconContainer : ContentControl
    {
        static IconContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconContainer), new FrameworkPropertyMetadata(typeof(IconContainer)));
        }

        public bool Visible
        {
            get { return (bool)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register(nameof(Visible), typeof(bool), typeof(IconContainer), new PropertyMetadata(true, (d, e) => ((IconContainer)d).VisibleChanged()));

        public TimeSpan FadeInOutDuration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(FadeInOutDuration), typeof(TimeSpan), typeof(IconContainer), new PropertyMetadata(TimeSpan.FromMilliseconds(250), (d, e) => ((IconContainer)d).DurationChanged()));


        DoubleAnimation fadeIn, fadeOut;


        public IconContainer()
        {
            fadeIn = new DoubleAnimation(1, FadeInOutDuration);
            fadeOut = new DoubleAnimation(0, FadeInOutDuration);
        }

        private void VisibleChanged()
        {
            if (Visible)
                BeginAnimation(OpacityProperty, fadeIn);
            else
                BeginAnimation(OpacityProperty, fadeOut);
        }

        private void DurationChanged()
        {
            fadeIn.Duration = FadeInOutDuration;
            fadeOut.Duration = FadeInOutDuration;
        }
    }
}
