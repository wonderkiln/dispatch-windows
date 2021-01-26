using Newtonsoft.Json;
using System.IO;

namespace Dispatch.Service.Storage
{
    public class Storage<T> : IStorage<T>
    {
        public string DirectoryPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Settings");

        public T Load(string fileName)
        {
            Directory.CreateDirectory(DirectoryPath);

            var path = Path.Combine(DirectoryPath, fileName);
            var text = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<T>(text);
        }

        public void Save(T value, string fileName)
        {
            Directory.CreateDirectory(DirectoryPath);

            var path = Path.Combine(DirectoryPath, fileName);
            var json = JsonConvert.SerializeObject(value);

            File.WriteAllText(path, json);
        }
    }
}
