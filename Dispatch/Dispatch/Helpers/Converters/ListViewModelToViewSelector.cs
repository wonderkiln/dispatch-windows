using Dispatch.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Helpers.Converters
{
    public class ListViewModelToViewSelector : DataTemplateSelector
    {
        public DataTemplate ListTemplate { get; set; }
        public DataTemplate ConnectTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ListViewModel)
            {
                return ListTemplate;
            }

            return ConnectTemplate;
        }
    }
}
