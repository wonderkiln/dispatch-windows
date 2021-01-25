using Dispatch.Service.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Dispatch.Helpers.Converters
{
    public class ResourceToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resource = value as Resource;

            switch (resource.Type)
            {
                case ResourceType.Drive:
                    return FileIconHelper.GetDriveIcon();
                case ResourceType.Directory:
                    return FileIconHelper.GetDirectoryIcon();
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
