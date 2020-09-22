using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Client
{
    public class FTPResource : IResource
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public bool Directory { get; set; }

        public long? Size { get; set; }

        public IClient Client { get; set; }

        public string CombinePath(string path)
        {
            return Path + "/" + path;
        }
    }

    public class FTPClient : IClient, IProgress<FtpProgress>
    {
        private FtpClient client = new FtpClient();

        public event EventHandler<ClientProgress> OnProgressChange;

        public string RootPath { get; private set; }

        private FTPResource MakeResource(FtpListItem item)
        {
            var resource = new FTPResource() { Path = item.FullName, Name = item.Name, Client = this };

            switch (item.Type)
            {
                case FtpFileSystemObjectType.Directory:
                    resource.Directory = true;
                    break;
                default:
                    resource.Size = item.Size;
                    resource.Directory = false;
                    break;
            }

            return resource;
        }

        public async Task Connect(string host, int port, string username, string password, string root)
        {
            client.SslProtocols = System.Security.Authentication.SslProtocols.Tls;
            client.ValidateAnyCertificate = true;
            client.DataConnectionType = FtpDataConnectionType.PASV;
            client.DownloadDataType = FtpDataType.Binary;
            client.RetryAttempts = 5;
            client.SocketPollInterval = 1000;
            client.ConnectTimeout = 2000;
            client.ReadTimeout = 2000;
            client.DataConnectionConnectTimeout = 2000;
            client.DataConnectionReadTimeout = 2000;

            //FtpTrace.EnableTracing = false;

            client.Host = host;
            client.Port = port;
            client.Credentials = new NetworkCredential(username, password);

            await client.ConnectAsync();

            RootPath = root;
        }

        public async Task Disconnect()
        {
            await client.DisconnectAsync();
        }

        public async Task<IResource> Resource(string path)
        {
            var item = await client.GetObjectInfoAsync(path);
            return MakeResource(item);
        }

        public async Task<List<IResource>> Resources(string path)
        {
            var items = await client.GetListingAsync(path);
            return items.Select(MakeResource).Cast<IResource>().ToList();
        }

        public async Task Delete(string path)
        {
            var resource = await Resource(path);

            if (resource.Directory)
            {
                await client.DeleteDirectoryAsync(resource.Path, FtpListOption.Auto);
            }
            else
            {
                await client.DeleteFileAsync(resource.Path);
            }
        }

        public async Task DownloadDirectory(string source, string destination, CancellationToken token = default)
        {
            await client.DownloadDirectoryAsync(destination, source, FtpFolderSyncMode.Update, FtpLocalExists.Overwrite, FtpVerify.None, null, this, token);
        }

        public async Task DownloadFile(string source, string destination, CancellationToken token)
        {
            await client.DownloadFileAsync(destination, source, FtpLocalExists.Overwrite, FtpVerify.None, this, token = default);
        }

        public async Task UploadDirectory(string source, string destination, CancellationToken token)
        {
            await client.UploadDirectoryAsync(source, destination, FtpFolderSyncMode.Update, FtpRemoteExists.Overwrite, FtpVerify.None, null, this, token = default);
        }

        public async Task UploadFile(string source, string destination, CancellationToken token)
        {
            await client.UploadFileAsync(source, destination, FtpRemoteExists.Overwrite, false, FtpVerify.None, this, token = default);
        }

        public void Report(FtpProgress value)
        {
            var progress = 100 * (value.FileIndex + value.Progress / 100) / value.FileCount;
            OnProgressChange?.Invoke(this, new ClientProgress() { TotalProgress = progress });
        }
    }
}
