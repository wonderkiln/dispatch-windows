namespace Dispatch.Service.Models
{
    public class S3Connection
    {
        public string Server { get; private set; }

        public string Key { get; private set; }

        public string Secret { get; private set; }

        public string Root { get; private set; }

        public S3Connection(string server, string key, string secret, string root)
        {
            Server = server;
            Key = key;
            Secret = secret;
            Root = string.IsNullOrEmpty(root) ? "/" : root;
        }

        public override string ToString()
        {
            return Server;
        }
    }
}
