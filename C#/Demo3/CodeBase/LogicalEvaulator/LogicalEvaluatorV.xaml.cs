using System.Windows;
using System.Windows.Controls;


namespace LogicalEvaluator
{
    public partial class LogicalEvaluatorV : UserControl
    {
        LogicalEvaluatorDC dc => (LogicalEvaluatorDC)DataContext;

        public LogicalEvaluatorV()
        {
            InitializeComponent();
        }

        private void AddModuleButtonClick(object sender, RoutedEventArgs e) => dc.AddSelectedElement();
        private void AddGroupButtonClick(object sender, RoutedEventArgs e) => dc.AddNewGroup();
        private void RemoveSelectedButtonClick(object sender, RoutedEventArgs e) => dc.RemoveSelectedElement();
        private void RemoveAllButtonClick(object sender, RoutedEventArgs e) => dc.RemoveAllElement();
    }
}
