using Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;


namespace UniCamApp.Control.Range
{
    /// <summary>
    /// Interaction logic for RangeSlider.xaml
    /// </summary>
    public partial class RangeSlider : UserControl
    {
        #region DP regs

        public static readonly DependencyProperty MaximumLimitProperty =
            DependencyProperty.Register(nameof(MaximumLimit), typeof(int), typeof(RangeSlider), new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Changed));

        public static readonly DependencyProperty MaximumRangeProperty =
            DependencyProperty.Register(nameof(MaximumRange), typeof(int), typeof(RangeSlider), new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Changed));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(RangeSlider), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Changed));

        public static readonly DependencyProperty MinimumRangeProperty =
            DependencyProperty.Register(nameof(MinimumRange), typeof(int), typeof(RangeSlider), new FrameworkPropertyMetadata(-5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Changed));

        public static readonly DependencyProperty MinimumLimitProperty =
            DependencyProperty.Register(nameof(MinimumLimit), typeof(int), typeof(RangeSlider), new FrameworkPropertyMetadata(-10, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Changed));

        public static readonly DependencyProperty IsValueInRangeProperty =
            DependencyProperty.Register(nameof(IsValueInRange), typeof(bool), typeof(RangeSlider), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        #region DP Props

        public int MaximumLimit
        {
            get { return (int)GetValue(MaximumLimitProperty); }
            set { SetValue(MaximumLimitProperty, value); }
        }

        public int MaximumRange
        {
            get { return (int)GetValue(MaximumRangeProperty); }
            set { SetValue(MaximumRangeProperty, value); }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int MinimumRange
        {
            get { return (int)GetValue(MinimumRangeProperty); }
            set { SetValue(MinimumRangeProperty, value); }
        }

        public int MinimumLimit
        {
            get { return (int)GetValue(MinimumLimitProperty); }
            set { SetValue(MinimumLimitProperty, value); }
        }

        public bool IsValueInRange
        {
            get { return (bool)GetValue(IsValueInRangeProperty); }
            set { SetValue(IsValueInRangeProperty, value); }
        }

        #endregion


        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RangeSlider)d).Draw();
        private void comboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => Draw();


        public RangeSlider()
        {
            InitializeComponent();

            comboBox.ItemsSource = new string[] { "tartomány", "ha nagyobb", "ha kisebb" };
            comboBox.SelectedItem = ((IEnumerable<string>)comboBox.ItemsSource).ElementAt(0);

            SizeChanged += (s, e) => Draw();
        }

        private void Draw()
        {
            double horizontalCenter = canvas.ActualHeight / 2;
            Canvas.SetTop(line1, horizontalCenter - (line1.Height / 2));
            Canvas.SetTop(line2, horizontalCenter - (line2.Height / 2));
            Canvas.SetLeft(line1, 0);
            Canvas.SetRight(line2, 0);
            Canvas.SetTop(slider1, horizontalCenter - (slider1.Height / 2));
            Canvas.SetTop(slider2, horizontalCenter - (slider2.Height / 2));

            int offset = 0 - MinimumLimit;
            int offsettedValue = offset + Value;
            int offsettedMaximumLimit = offset + MaximumLimit;
            double valuePercentage = (double)offsettedValue / (double)offsettedMaximumLimit;

            line1.Width = LimitBetweenZeroAndActualWidth(ActualWidth * valuePercentage);
            line2.Width = LimitBetweenZeroAndActualWidth(ActualWidth - (ActualWidth * valuePercentage));

            if((string)comboBox.SelectedItem == ((IEnumerable<string>)comboBox.ItemsSource).ElementAt(0))
            {
                slider1.Visibility = Visibility.Visible;
                slider2.Visibility = Visibility.Visible;

                if (Value >= MinimumRange && Value <= MaximumRange)
                {
                    line1.Fill = Brushes.LightGreen;
                    IsValueInRange = true;
                }
                else
                {
                    line1.Fill = Brushes.Red;
                    IsValueInRange = false;
                }
            }

            else if ((string)comboBox.SelectedItem == ((IEnumerable<string>)comboBox.ItemsSource).ElementAt(1))
            {
                slider1.Visibility = Visibility.Visible;
                slider2.Visibility = Visibility.Hidden;

                if (Value >= MinimumRange)
                {
                    line1.Fill = Brushes.LightGreen;
                    IsValueInRange = true;
                }
                else
                {
                    line1.Fill = Brushes.Red;
                    IsValueInRange = false;
                }
            }

            else if ((string)comboBox.SelectedItem == ((IEnumerable<string>)comboBox.ItemsSource).ElementAt(2))
            {
                slider1.Visibility = Visibility.Hidden;
                slider2.Visibility = Visibility.Visible;

                if (Value <= MaximumRange)
                {
                    line1.Fill = Brushes.LightGreen;
                    IsValueInRange = true;
                }
                else
                {
                    line1.Fill = Brushes.Red;
                    IsValueInRange = false;
                }
            }

            int offsettedMinimumRange = offset + MinimumRange;
            double minimumRangePercentage = (double)offsettedMinimumRange / (double)offsettedMaximumLimit;
            Canvas.SetLeft(slider1, LimitBetweenZeroAndActualWidth(ActualWidth * minimumRangePercentage));

            int offsettedMaximumRange = offset + MaximumRange;
            double maximumRangePercentage = (double)offsettedMaximumRange / (double)offsettedMaximumLimit;
            Canvas.SetLeft(slider2, LimitBetweenZeroAndActualWidth(ActualWidth * maximumRangePercentage));
        }

        private double LimitBetweenZeroAndActualWidth(double value)
        {
            if (value < 0)
                return 0;
            
            if (value > ActualWidth)
                return ActualWidth;

            return value;
        }

        private void Slider1MouseDown(object sender, MouseButtonEventArgs e) => Mouse.Capture(slider1);
        private void Slider2MouseDown(object sender, MouseButtonEventArgs e) => Mouse.Capture(slider2);
        private void SliderMouseUp(object sender, MouseButtonEventArgs e) => Mouse.Capture(null);

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if(Mouse.Captured.IsNotNull())
            {
                double hPosition = Mouse.GetPosition((Canvas)sender).X;

                if (hPosition < 0)
                    hPosition = 0;

                if (hPosition > ActualWidth)
                    hPosition = ActualWidth;

                int offset = 0 - MinimumLimit;
                int offsettedMaximumLimit = offset + MaximumLimit;

                if (Mouse.Captured == slider1)
                {
                    double slider1NewPosition = hPosition - (slider1.Width / 2);
                    double minimumRangePercentage = slider1NewPosition / ActualWidth;
                    double offsettedMinimumRange = (double)minimumRangePercentage * (double)offsettedMaximumLimit;
                    int minimumRange = (int)Math.Round(offsettedMinimumRange + MinimumLimit, 0);

                    if (minimumRange < MinimumLimit)
                        minimumRange = MinimumLimit;

                    if (minimumRange > MaximumRange)
                        minimumRange = MaximumRange;

                    MinimumRange = minimumRange;
                }

                if (Mouse.Captured == slider2)
                {
                    double slider2NewPosition = hPosition - (slider2.Width / 2);
                    double maximumRangePercentage = slider2NewPosition / ActualWidth;
                    double offsettedMaximumRange = (double)maximumRangePercentage * (double)offsettedMaximumLimit;
                    int maximumRange = (int)Math.Round(offsettedMaximumRange + MinimumLimit, 0);

                    if (maximumRange > MaximumLimit)
                        maximumRange = MaximumLimit;

                    if (maximumRange < MinimumRange)
                        maximumRange = MinimumRange;

                    MaximumRange = maximumRange;
                }
            }
        }
    }
}
