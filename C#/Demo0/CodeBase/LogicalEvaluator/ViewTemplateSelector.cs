using System.Windows;
using System.Windows.Controls;


namespace LogicalEvaluator
{
    public class ViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ElementTemplate { get; set; }
        public DataTemplate GroupTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType() == typeof(LogicalEvaluatorDC))
                return GroupTemplate;

            return ElementTemplate;
        }
    }
}
