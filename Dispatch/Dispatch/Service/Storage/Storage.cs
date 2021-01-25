using Newtonsoft.Json;
using System.IO;

namespace Dispatch.Service.Storage
{
    public class Storage<T> : IStorage<T>
    {
        public T Load(string path)
        {
            var text = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(text);
        }

        public void Save(T value, string path)
        {
            var json = JsonConvert.SerializeObject(value);
            File.WriteAllText(path, json);
        }
    }
}
