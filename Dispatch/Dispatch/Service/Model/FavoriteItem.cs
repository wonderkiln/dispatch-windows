namespace Dispatch.Service.Model
{
    public class FavoriteItem
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public FavoriteItem(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
