using Dispatch.Service.Model;
using FluentFTP;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public class FTPClient : IClient
    {
        private readonly FtpClient Client = new FtpClient();

        public string Name { get; private set; } = "";

        public string InitialPath { get; private set; } = "/";

        public FTPClient(FtpClient client, string path)
        {
            Client = client;

            Name = $"{client.Host}:{client.Port}";

            if (!string.IsNullOrEmpty(path))
            {
                InitialPath = path;
            }
        }

        public static async Task<FTPClient> Create(string host, int port, string username, string password, string path = null)
        {
            FtpTrace.EnableTracing = false;

            var client = new FtpClient();

            client.SslProtocols = SslProtocols.Tls;
            client.ValidateAnyCertificate = true;
            client.DataConnectionType = FtpDataConnectionType.PASV;
            client.DownloadDataType = FtpDataType.Binary;
            //Client.RetryAttempts = 5;
            //Client.SocketPollInterval = 1000;
            //Client.ConnectTimeout = 2000;
            //Client.ReadTimeout = 2000;
            //Client.DataConnectionConnectTimeout = 2000;
            //Client.DataConnectionReadTimeout = 2000;

            client.Host = host;
            client.Port = port;
            client.Credentials = new NetworkCredential(username, password);

            await client.ConnectAsync();

            return new FTPClient(client, path);
        }

        public async Task<IClient> Clone()
        {
            return await FTPClient.Create(Client.Host, Client.Port, Client.Credentials.UserName, Client.Credentials.Password);
        }

        public async Task Diconnect()
        {
            await Client.DisconnectAsync();
            Client.Dispose();
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

        public async Task Delete(string path, CancellationToken token = default)
        {
            var resource = await FetchResource(path);

            if (resource.Type == ResourceType.Directory)
            {
                await Client.DeleteDirectoryAsync(path, token);
            }
            else
            {
                await Client.DeleteFileAsync(path, token);
            }
        }

        private class FtpProgressConverter : IProgress<FtpProgress>
        {
            private readonly IProgress<ProgressStatus> progress;

            public FtpProgressConverter(IProgress<ProgressStatus> progress)
            {
                this.progress = progress;
            }

            public void Report(FtpProgress value)
            {
                progress?.Report(new ProgressStatus(value.FileIndex, value.FileCount, value.Progress));
            }
        }

        public async Task Upload(string path, string fileOrDirectory, IProgress<ProgressStatus> progress = null, CancellationToken token = default)
        {
            var normalizedPath = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;

            if (File.Exists(fileOrDirectory))
            {
                var destination = $"{normalizedPath}/{Path.GetFileName(fileOrDirectory)}";
                await Client.UploadFileAsync(fileOrDirectory, destination, FtpRemoteExists.Overwrite, false, FtpVerify.None, new FtpProgressConverter(progress), token);
            }
            else if (Directory.Exists(fileOrDirectory))
            {
                var destination = $"{normalizedPath}/{Path.GetFileName(fileOrDirectory)}";
                await Client.UploadDirectoryAsync(fileOrDirectory, destination, FtpFolderSyncMode.Update, FtpRemoteExists.Skip, FtpVerify.None, null, new FtpProgressConverter(progress), token);
            }
            else
            {
                throw new Exception($"File or directory not found at path: {fileOrDirectory}");
            }
        }
    }
}
