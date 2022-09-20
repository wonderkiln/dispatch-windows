using Dispatch.Service.Models;
using FluentFTP;
using FluentFTP.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public class FTPClient : IClient
    {
        private readonly AsyncFtpClient client;

        private readonly FTPConnection connectionInfo;

        public FTPClient(AsyncFtpClient client, FTPConnection connectionInfo)
        {
            this.client = client;
            this.connectionInfo = connectionInfo;
        }

        public static async Task<FTPClient> Create(FTPConnection connectionInfo)
        {
            var client = new AsyncFtpClient();
            client.Config.ValidateAnyCertificate = true;
            client.Config.DataConnectionType = FtpDataConnectionType.PASV;
            client.Host = connectionInfo.Address;
            client.Port = connectionInfo.Port;
            client.Credentials = new NetworkCredential(connectionInfo.Username, connectionInfo.Password);

            await client.Connect();

            return new FTPClient(client, connectionInfo);
        }

        public async Task<IClient> Clone()
        {
            return await Create(connectionInfo);
        }

        public async Task Diconnect()
        {
            await client.Disconnect();
            client.Dispose();
        }

        private Resource MakeResource(FtpListItem item)
        {
            switch (item.Type)
            {
                case FtpObjectType.Directory:
                    return new Resource(this, item.FullName, item.Name)
                    {
                        Type = ResourceType.Directory,
                    };
                default:
                    return new Resource(this, item.FullName, item.Name)
                    {
                        Type = ResourceType.File,
                        Size = item.Size,
                        Modified = item.Modified,
                    };
            }
        }

        public async Task<Resource> FetchResource(string path)
        {
            var item = await client.GetObjectInfo(path);

            if (item == null)
            {
                throw new Exception($"Resource not found at path: {path}");
            }

            return MakeResource(item);
        }

        public async Task<Resource[]> FetchResources(string path)
        {
            var items = await client.GetListing(path);
            return items.Select(MakeResource).ToArray();
        }

        public async Task Delete(string path, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var resource = await FetchResource(path);

            if (resource.Type == ResourceType.Directory)
            {
                await client.DeleteDirectory(path, token);
            }
            else
            {
                await client.DeleteFile(path, token);
            }
        }

        private class FtpProgressConverter : IProgress<FtpProgress>
        {
            private readonly IProgress<ResourceProgress> progress;

            public bool Uploading { get; set; }

            public FtpProgressConverter(IProgress<ResourceProgress> progress)
            {
                this.progress = progress;
            }

            public void Report(FtpProgress value)
            {
                string path = null;
                if (value.FileCount > 1) path = Uploading ? value.LocalPath : value.RemotePath;
                progress?.Report(new ResourceProgress(value.FileIndex, value.FileCount, value.Progress, path));
            }
        }

        public async Task Upload(string path, string fileOrDirectory, IProgress<ResourceProgress> progress = null, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var normalizedPath = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;

            if (File.Exists(fileOrDirectory))
            {
                var destination = $"{normalizedPath}/{Path.GetFileName(fileOrDirectory)}";
                await client.UploadFile(fileOrDirectory, destination, FtpRemoteExists.Overwrite, false, FtpVerify.None, new FtpProgressConverter(progress) { Uploading = true }, token);
            }
            else if (Directory.Exists(fileOrDirectory))
            {
                var destination = $"{normalizedPath}/{Path.GetFileName(fileOrDirectory)}";
                await client.UploadDirectory(fileOrDirectory, destination, FtpFolderSyncMode.Update, FtpRemoteExists.Skip, FtpVerify.None, null, new FtpProgressConverter(progress) { Uploading = true }, token);
            }
            else
            {
                throw new Exception($"File or directory not found at path: {fileOrDirectory}");
            }
        }

        public async Task Download(string path, string toDirectory, IProgress<ResourceProgress> progress = null, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var resource = await FetchResource(path);

            var localPath = Path.Combine(toDirectory, resource.Name);

            if (resource.Type == ResourceType.File)
            {
                await client.DownloadFile(localPath, path, FtpLocalExists.Overwrite, FtpVerify.None, new FtpProgressConverter(progress) { Uploading = false }, token);
            }
            else if (resource.Type == ResourceType.Directory)
            {
                await client.DownloadDirectory(localPath, path, FtpFolderSyncMode.Update, FtpLocalExists.Skip, FtpVerify.None, null, new FtpProgressConverter(progress) { Uploading = false }, token);
            }
        }
    }
}
