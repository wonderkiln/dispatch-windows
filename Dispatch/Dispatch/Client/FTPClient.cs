using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Client
{
    public class FTPClient : IClient
    {
        public string Name { get; private set; } = "Remote";

        public string Root { get; private set; }

        private FtpClient client = new FtpClient();

        public async Task Connect(string host, int port, string username, string password, string root)
        {
            client.Host = host;
            client.Port = port;
            client.Credentials = new NetworkCredential(username, password);

            await client.ConnectAsync();

            Root = root;
            Name = host; // or custom name
        }

        public async Task Disconnect()
        {
            await client.DisconnectAsync();
        }

        public async Task<List<Resource>> List(string path)
        {
            var list = await client.GetListingAsync(path);

            return list.Select(e =>
            {
                var item = new Resource() { Path = e.FullName, Name = e.Name, Client = this };

                switch (e.Type)
                {
                    case FtpFileSystemObjectType.File:
                        item.Size = e.Size;
                        item.Type = ResourceType.File;
                        break;
                    case FtpFileSystemObjectType.Link:
                        item.Size = e.Size;
                        item.Type = ResourceType.Link;
                        break;
                    case FtpFileSystemObjectType.Directory:
                        item.Type = ResourceType.Directory;
                        break;
                }

                return item;
            }).ToList();
        }

        public async Task<string> Download(Resource resource, string destination)
        {
            if (resource.Type == ResourceType.Directory)
            {
                await client.DownloadDirectoryAsync(destination, resource.Path);

                return destination;
            }
            else
            {
                var path = Path.Combine(destination, resource.Name);
                await client.DownloadFileAsync(path, resource.Path);

                return path;
            }
        }

        // TODO: resource
        public async Task Upload(string source, string destination)
        {
            var fileInfo = new FileInfo(source);

            if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                await client.UploadDirectoryAsync(source, destination);
            }
            else
            {
                await client.UploadFileAsync(source, destination + "/" + fileInfo.Name);
            }
        }
    }
}
