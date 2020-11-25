using Dispatch.Service.Client;

namespace Dispatch.Service.Model
{
    public enum ResourceType { Directory, File, Drive }

    public class Resource
    {
        public IClient Client { get; private set; }

        public string Path { get; private set; }

        public string Name { get; private set; }

        public ResourceType Type { get; set; }

        public long? Size { get; set; }

        public Resource(IClient client, string path, string name)
        {
            Client = client;
            Path = path;
            Name = name;
        }
    }
}
