using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Client
{
    public class FTPClient : IClient
    {
        public string Root { get; private set; }

        private FtpClient client = new FtpClient();

        public async Task Connect(string host, int port, string username, string password, string root)
        {
            client.Host = host;
            client.Port = port;
            client.Credentials = new NetworkCredential(username, password);
            
            await client.ConnectAsync();

            Root = root;
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
                var item = new Resource()
                {
                    Path = e.FullName,
                    Name = e.Name,
                    Size = e.Size
                };

                switch (e.Type)
                {
                    case FtpFileSystemObjectType.File:
                        item.Type = ResourceType.File;
                        break;
                    case FtpFileSystemObjectType.Link:
                        item.Type = ResourceType.Link;
                        break;
                    case FtpFileSystemObjectType.Directory:
                        item.Type = ResourceType.Directory;
                        break;
                }

                return item;
            }).ToList();
        }
    }
}
