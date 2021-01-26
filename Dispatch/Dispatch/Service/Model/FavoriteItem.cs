using Dispatch.View.Fragments;

namespace Dispatch.Service.Model
{
    public class FavoriteItem
    {
        public string Title { get; set; }

        public ConnectViewType Type { get; set; }

        public object ConnectionInfo { get; set; }
    }
}
