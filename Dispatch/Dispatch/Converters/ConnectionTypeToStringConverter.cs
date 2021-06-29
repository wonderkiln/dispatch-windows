using System;
using System.Globalization;
using System.Windows.Data;
using static Dispatch.Service.Models.Favorite;

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
                case ConnectionType.S3:
                    return "Amazon S3";
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
