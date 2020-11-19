using Dispatch.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Dispatch.Helpers
{
    public class PathToPathComponentsConverter : IValueConverter
    {
        public class PathComponent
        {
            public string Name { get; set; }

            public string Path { get; set; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resource = value as IResource;

            if (resource == null)
            {
                return null;
            }

            var components = resource.Path
                .Split(resource.PathSeparator)
                .Where(e => !string.IsNullOrEmpty(e))
                .Select(e => new PathComponent() { Name = e, Path = e })
                .ToArray();

            for (int i = 1; i < components.Length; i++)
            {
                components[i].Path = components[i - 1].Path + resource.PathSeparator + components[i].Path;
            }

            return components;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
