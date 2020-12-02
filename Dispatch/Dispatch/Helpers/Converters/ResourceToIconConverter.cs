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
                case ResourceType.Directory:
                    return "\xE8B7";
                case ResourceType.File:
                    return "\xE7C3";
                case ResourceType.Drive:
                    return "\xEDA2";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
