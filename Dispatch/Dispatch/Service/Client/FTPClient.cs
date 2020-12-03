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

        public FTPClient(string path)
        {
            Client.SslProtocols = SslProtocols.Tls;
            Client.ValidateAnyCertificate = true;
            Client.DataConnectionType = FtpDataConnectionType.PASV;
            Client.DownloadDataType = FtpDataType.Binary;
            //Client.RetryAttempts = 5;
            //Client.SocketPollInterval = 1000;
            //Client.ConnectTimeout = 2000;
            //Client.ReadTimeout = 2000;
            //Client.DataConnectionConnectTimeout = 2000;
            //Client.DataConnectionReadTimeout = 2000;
            
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

            Name = $"{host}:{port}";
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
            // Create a new connection for this upload
            var client = new FtpClient(Client.Host, Client.Port, Client.Credentials);
            client.SslProtocols = SslProtocols.Tls;
            client.ValidateAnyCertificate = true;
            client.DataConnectionType = FtpDataConnectionType.PASV;
            client.DownloadDataType = FtpDataType.Binary;

            await client.ConnectAsync();
            
            var normalizedPath = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;

            if (File.Exists(fileOrDirectory))
            {
                var destination = $"{normalizedPath}/{Path.GetFileName(fileOrDirectory)}";
                await client.UploadFileAsync(fileOrDirectory, destination, FtpRemoteExists.Overwrite, false, FtpVerify.None, new FtpProgressConverter(progress), token);
            }
            else if (Directory.Exists(fileOrDirectory))
            {
                var destination = $"{normalizedPath}/{Path.GetFileName(fileOrDirectory)}";
                await client.UploadDirectoryAsync(fileOrDirectory, destination, FtpFolderSyncMode.Update, FtpRemoteExists.Skip, FtpVerify.None, null, new FtpProgressConverter(progress), token);
            }
            else
            {
                throw new Exception($"File or directory not found at path: {fileOrDirectory}");
            }

            client.Dispose();
        }
    }
}
