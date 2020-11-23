﻿using ByteSizeLib;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Dispatch.Helpers
{
    public class BytesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return ByteSize.FromBytes((long)value).ToString();
            }

            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
