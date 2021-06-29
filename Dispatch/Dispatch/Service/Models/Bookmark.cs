using Dispatch.Service.Client;

namespace Dispatch.Service.Models
{
    public class Bookmark
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsLocal { get; set; }

        public Bookmark() { }

        public Bookmark(Resource resource)
        {
            Name = resource.Name;
            Path = resource.Path;
            IsLocal = resource.Client is LocalClient;
        }

        public Bookmark(string name, string path, bool isLocal)
        {
            Name = name;
            Path = path;
            IsLocal = isLocal;
        }
    }
}
