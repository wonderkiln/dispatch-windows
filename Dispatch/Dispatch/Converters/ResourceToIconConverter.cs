using Dispatch.Helpers;
using Dispatch.Service.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Dispatch.Converters
{
    public class ResourceToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resource = value as Resource;

            switch (resource.Type)
            {
                case ResourceType.Drive:
                    return Icons.Drive;
                case ResourceType.Directory:
                    return Icons.Folder;
                case ResourceType.File:
                    return FileIconHelper.GetFileIcon(resource.Path);
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
