using Dispatch.Service.Client;
using System;

namespace Dispatch.Service.Models
{
    public enum ResourceType { Drive = 0, Directory = 1, File = 2 }

    public class Resource
    {
        public IClient Client { get; private set; }

        public string Path { get; private set; }

        public string Name { get; private set; }

        public ResourceType Type { get; set; }

        public long? Size { get; set; }

        public DateTime? Modified { get; set; }

        public Resource(IClient client, string path, string name)
        {
            Client = client;
            Path = path;
            Name = name;
        }
    }
}
