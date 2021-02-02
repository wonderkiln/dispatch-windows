using System;
using System.Globalization;
using System.Windows.Data;
using static Dispatch.Service.Model.Favorite;

namespace Dispatch.Converters
{
    public class ConnectionTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (ConnectionType)value;

            switch (type)
            {
                case ConnectionType.Sftp:
                    return "SFTP";
                case ConnectionType.Ftp:
                    return "FTP";
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
