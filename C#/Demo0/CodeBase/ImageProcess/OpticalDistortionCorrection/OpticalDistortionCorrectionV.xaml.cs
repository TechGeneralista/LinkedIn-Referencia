using System.Windows;
using System.Windows.Controls;


namespace ImageProcess.OpticalDistortionCorrection
{
    /// <summary>
    /// Interaction logic for OpticalDistortionCorrectionV.xaml
    /// </summary>
    public partial class OpticalDistortionCorrectionV : UserControl
    {
        public OpticalDistortionCorrectionV()
        {
            InitializeComponent();
        }

        private void SetAllToDefaultButtonClick(object sender, RoutedEventArgs e) => ((OpticalDistortionCorrectionDC)DataContext).ResetAllToDefault();
    }
}
