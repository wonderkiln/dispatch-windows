using Dispatch.Service.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Dispatch.Converters
{
    public class ThemeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (AppTheme)value;

            switch (type)
            {
                case AppTheme.Auto:
                    return "\uE771";
                case AppTheme.Light:
                    return "\uE706";
                case AppTheme.Dark:
                    return "\uE708";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
