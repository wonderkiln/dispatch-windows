namespace Dispatch.Service.Model
{
    public class FavoriteItem
    {
        public enum ConnectionType { Sftp, Ftp }

        public string Title { get; set; }

        public ConnectionType Connection { get; set; }

        public object ConnectionInfo { get; set; }
    }
}
