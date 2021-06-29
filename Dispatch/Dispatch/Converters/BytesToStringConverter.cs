using ByteSizeLib;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Dispatch.Converters
{
    public class BytesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long bytes)
            {
                return ByteSize.FromBytes(bytes).ToString();
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
