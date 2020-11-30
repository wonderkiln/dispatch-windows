using Dispatch.Service.Model;
using FluentFTP;
using System;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public class FTPClient : IClient
    {
        private readonly FtpClient Client = new FtpClient();

        public string Title { get; private set; } = "";

        public string InitialPath { get; private set; } = "/";

        public FTPClient(string path)
        {
            Client.SslProtocols = SslProtocols.Tls;
            Client.ValidateAnyCertificate = true;
            Client.DataConnectionType = FtpDataConnectionType.PASV;
            Client.DownloadDataType = FtpDataType.Binary;
            Client.RetryAttempts = 5;
            Client.SocketPollInterval = 1000;
            Client.ConnectTimeout = 2000;
            Client.ReadTimeout = 2000;
            Client.DataConnectionConnectTimeout = 2000;
            Client.DataConnectionReadTimeout = 2000;

            FtpTrace.EnableTracing = false;

            if (!string.IsNullOrEmpty(path))
            {
                InitialPath = path;
            }
        }

        public async Task Connect(string host, int port, string username, string password)
        {
            Client.Host = host;
            Client.Port = port;
            Client.Credentials = new NetworkCredential(username, password);

            await Client.ConnectAsync();

            Title = $"{host}:{port}";
        }

        public async Task Diconnect()
        {
            await Client.DisconnectAsync();
        }

        private Resource MakeResource(FtpListItem item)
        {
            switch (item.Type)
            {
                case FtpFileSystemObjectType.Directory:
                    return new Resource(this, item.FullName, item.Name)
                    {
                        Type = ResourceType.Directory,
                    };
                default:
                    return new Resource(this, item.FullName, item.Name)
                    {
                        Type = ResourceType.File,
                        Size = item.Size,
                    };
            }
        }

        public async Task<Resource> FetchResource(string path)
        {
            var item = await Client.GetObjectInfoAsync(path);
            
            if (item == null)
            {
                throw new Exception($"Resource not found at path: {path}");
            }

            return MakeResource(item);
        }

        public async Task<Resource[]> FetchResources(string path)
        {
            var items = await Client.GetListingAsync(path);
            return items.Select(MakeResource).ToArray();
        }

        public async Task Delete(string path)
        {
            var resource = await FetchResource(path);

            if (resource.Type == ResourceType.Directory)
            {
                await Client.DeleteDirectoryAsync(path);
            }
            else
            {
                await Client.DeleteFileAsync(path);
            }
        }
    }
}
