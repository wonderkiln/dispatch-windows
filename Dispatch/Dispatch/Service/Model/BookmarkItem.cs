using Dispatch.Service.Client;

namespace Dispatch.Service.Model
{
    public class BookmarkItem
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsLocal { get; set; }

        public BookmarkItem() { }

        public BookmarkItem(Resource resource)
        {
            Name = resource.Name;
            Path = resource.Path;
            IsLocal = resource.Client is LocalClient;
        }

        public BookmarkItem(string name, string path, bool isLocal)
        {
            Name = name;
            Path = path;
            IsLocal = isLocal;
        }
    }
}
