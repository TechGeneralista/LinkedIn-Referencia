using Common.Tool;
using System.Windows;
using System.Windows.Controls;


namespace UserProgram
{
    /// <summary>
    /// Interaction logic for InterpreterView.xaml
    /// </summary>
    public partial class InterpreterView : UserControl
    {
        InterpreterViewModel interpreter => (InterpreterViewModel)DataContext;


        public InterpreterView()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<InterpreterViewModel>();
        }

        private void RunButtonClick(object sender, RoutedEventArgs e) => interpreter.RunButtonClick();
        private void StopButtonClick(object sender, RoutedEventArgs e) => interpreter.StopButtonClick();
    }
}
