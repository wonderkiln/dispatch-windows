using Dispatch.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Converters
{
    public class TabViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Connect { get; set; }
        public DataTemplate Resources { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ResourcesViewModel) return Resources;
            if (item is ConnectViewModel) return Connect;
            return null;
        }
    }
}
