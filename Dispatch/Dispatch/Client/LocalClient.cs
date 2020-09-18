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
        public string Name { get; } = "Local";

        public string Root { get; } = @"C:\Users\Admin\Downloads";

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task<string> Download(Resource resource, string destination)
        {
            return Task.FromResult(resource.Path);
        }

        public Task<List<Resource>> List(string path)
        {
            //if (path == Root)
            //{
            //    return Task.FromResult(DriveInfo.GetDrives().Select(e => new Resource() { Path = e.Name, Name = e.Name, Type = ResourceType.Directory, Client = this }).ToList());
            //}

            var directories = Directory.GetDirectories(path).Select(e => new Resource() { Path = e, Name = Path.GetFileName(e), Type = ResourceType.Directory, Client = this }).ToList();
            var files = Directory.GetFiles(path).Select(e => new Resource() { Path = e, Name = Path.GetFileName(e), Type = ResourceType.File, Size = new FileInfo(e).Length, Client = this }).ToList();

            return Task.FromResult(directories.Concat(files).ToList());
        }

        public Task Upload(string source, string destination)
        {
            return new Task(() =>
            {
                var fileInfo = new FileInfo(source);

                if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
                {
                    Directory.Move(source, destination);
                }
                else
                {
                    var path = Path.Combine(destination, Path.GetFileName(source));
                    File.Move(source, path);
                }
            });
        }
    }
}
