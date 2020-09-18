using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Client
{
    public class LocalClient : IClient
    {
        public string Root { get; } = "";

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task<List<Resource>> List(string path)
        {
            if (path == Root)
            {
                return Task.FromResult(DriveInfo.GetDrives().Select(e => new Resource() { Path = e.Name, Name = e.Name, Type = ResourceType.Directory }).ToList());
            }

            var directories = Directory.GetDirectories(path).Select(e => new Resource() { Path = e, Name = Path.GetFileName(e), Type = ResourceType.Directory }).ToList();
            var files = Directory.GetFiles(path).Select(e => new Resource() { Path = e, Name = Path.GetFileName(e), Type = ResourceType.File, Size = new FileInfo(e).Length }).ToList();

            return Task.FromResult(directories.Concat(files).ToList());
        }
    }
}
