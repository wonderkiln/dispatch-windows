using Newtonsoft.Json;
using System;
using System.IO;

namespace Dispatch.Service.Storage
{
    public class Storage<T>
    {
        public string DirectoryPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Settings");

        public string FileName { get; }

        public Storage(string fileName)
        {
            FileName = fileName;
        }

        public T Load()
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

            return (T)Activator.CreateInstance(typeof(T));
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
