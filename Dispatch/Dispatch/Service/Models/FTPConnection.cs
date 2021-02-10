namespace Dispatch.Service.Models
{
    public class FTPConnection
    {
        public string Address { get; private set; }

        public int Port { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public string Root { get; private set; }

        public FTPConnection(string address, int port, string username, string password, string root)
        {
            Address = address;
            Port = port;
            Username = username;
            Password = password;
            Root = root;
        }

        public override string ToString()
        {
            return $"{Username}@{Address}:{Port}";
        }
    }
}
