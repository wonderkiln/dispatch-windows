namespace Dispatch.Service.Model
{
    public class FTPConnectionInfo
    {
        public string Address { get; private set; }

        public int Port { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public string Root { get; private set; } = "/";

        public FTPConnectionInfo(string address, int port, string username, string password, string root)
        {
            Address = address;
            Port = port;
            Username = username;
            Password = password;

            if (!string.IsNullOrEmpty(root))
            {
                Root = root;
            }
        }

        public override string ToString()
        {
            return $"{Address}:{Port}";
        }
    }
}
