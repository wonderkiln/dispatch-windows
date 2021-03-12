using System;
using System.Globalization;
using System.Windows.Data;

namespace Dispatch.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                if (parameter is string format)
                {
                    return dateTime.ToString(format);
                }

                return dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
