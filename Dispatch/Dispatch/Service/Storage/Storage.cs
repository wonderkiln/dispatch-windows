using Dispatch.Helpers;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Dispatch.Service.Storage
{
    public class Storage<T>
    {
        public string DirectoryPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.APP_NAME, "Settings");

        public string FileName { get; }

        public Storage(string fileName)
        {
            FileName = fileName;
        }

        public T Load(T defaultValue = default)
        {
            try
            {
                Directory.CreateDirectory(DirectoryPath);

                var path = Path.Combine(DirectoryPath, FileName);
                var text = File.ReadAllText(path);

                return JsonConvert.DeserializeObject<T>(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return defaultValue;
        }

        public void Save(T value)
        {
            Directory.CreateDirectory(DirectoryPath);

            var path = Path.Combine(DirectoryPath, FileName);
            var json = JsonConvert.SerializeObject(value);

            File.WriteAllText(path, json);
        }
    }
}
