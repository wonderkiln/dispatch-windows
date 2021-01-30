using Dispatch.Service.Model;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Helpers.Converters
{
    public class ItemToTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Bookmark { get; set; }
        public DataTemplate Resource { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is BookmarkItem) return Bookmark;
            if (item is Resource) return Resource;
            return null;
        }
    }
}
