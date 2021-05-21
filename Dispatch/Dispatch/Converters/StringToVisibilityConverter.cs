using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dispatch.Converters
{
    class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string valueAsString && !string.IsNullOrEmpty(valueAsString))
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
