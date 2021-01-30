using Dispatch.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Helpers.Converters
{
    public class TabViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ConnectDataTemplate { get; set; }
        public DataTemplate ResourcesDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ResourcesViewModel)
            {
                return ResourcesDataTemplate;
            }

            return ConnectDataTemplate;
        }
    }
}
