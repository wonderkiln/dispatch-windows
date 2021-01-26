namespace Dispatch.Service.Model
{
    public class BookmarkItem
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public BookmarkItem() { }

        public BookmarkItem(Resource resource)
        {
            Name = resource.Name;
            Path = resource.Path;
        }

        public BookmarkItem(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
