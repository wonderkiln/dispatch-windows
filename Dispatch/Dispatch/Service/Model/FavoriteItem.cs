namespace Dispatch.Service.Model
{
    public class FavoriteItem
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public FavoriteItem()
        {
            //
        }

        public FavoriteItem(Resource resource)
        {
            Name = resource.Name;
            Path = resource.Path;
        }

        public FavoriteItem(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
