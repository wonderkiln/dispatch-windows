namespace Dispatch.Service.Models
{
    public class SFTPConnection
    {
        public string Address { get; private set; }

        public int Port { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public string Key { get; private set; }

        public string Root { get; private set; }

        public SFTPConnection(string address, int port, string username, string password, string key, string root)
        {
            Address = address;
            Port = port;
            Username = username;
            Password = password;
            Key = key;
            Root = root;
        }

        public override string ToString()
        {
            return $"{Username}@{Address}:{Port}";
        }
    }
}
