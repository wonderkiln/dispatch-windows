using Dispatch.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;
using static Dispatch.Service.Models.Favorite;

namespace Dispatch.Converters
{
    public class ConnectionTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (ConnectionType)value;

            switch (type)
            {
                case ConnectionType.Sftp:
                    return Icons.Sftp;
                case ConnectionType.Ftp:
                    return Icons.Ftp;
                case ConnectionType.S3:
                    return Icons.S3;
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
