using Dispatch.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dispatch.Service.Theme
{
    public partial class FileIconTheme
    {

        private static Dictionary<string, ImageSource> ExtensionImageMap = new Dictionary<string, ImageSource>();

        public static event EventHandler<EventArgs> OnInvalidate;

        public static ThemePackage LoadThemeMetadata(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                {
                    var package = archive.GetEntry("package.json");

                    if (package == null)
                    {
                        throw new Exception("package.json was not found");
                    }

                    using (var stream = package.Open())
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            using (var jsonReader = new JsonTextReader(streamReader))
                            {
                                var serializer = new JsonSerializer();
                                var result = serializer.Deserialize<ThemePackage>(jsonReader);
                                result.Path = path;
                                return result;
                            }
                        }
                    }
                }
            }
        }

        public static void LoadTheme(string path)
        {
            ExtensionImageMap.Clear();

            var package = LoadThemeMetadata(path);

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                {
                    foreach (var fileIcon in package.FileIcons)
                    {
                        var image = archive.GetEntry(fileIcon.Source);

                        if (image == null)
                        {
                            throw new Exception($"{fileIcon.Source} was not found");
                        }

                        var memoryStream = new MemoryStream();

                        using (var imageStream = image.Open())
                        {
                            imageStream.CopyTo(memoryStream);
                        }

                        var bitmapImage = new BitmapImage
                        {
                            CacheOption = BitmapCacheOption.OnDemand,
                            CreateOptions = BitmapCreateOptions.DelayCreation
                        };

                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();

                        foreach (var extension in fileIcon.Extensions)
                        {
                            if (!ExtensionImageMap.ContainsKey(extension))
                            {
                                ExtensionImageMap.Add(extension, bitmapImage);
                            }
                        }
                    }
                }
            }

            OnInvalidate?.Invoke(null, EventArgs.Empty);
        }

        public static ImageSource GetImageSourceAt(string key)
        {
            if (ExtensionImageMap.TryGetValue(key, out ImageSource imageSource))
            {
                return imageSource;
            }

            return null;
        }

        public static ImageSource GetFileImageSource(string path)
        {
            var extension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension)) return GetImageSourceAt("*");

            var key = extension.Substring(1).ToLower();
            return GetImageSourceAt(key) ?? GetImageSourceAt("*");
        }

        public static ImageSource GetFolderImageSource()
        {
            return GetImageSourceAt("_");
        }
    }
}
