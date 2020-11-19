using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Client
{
    public class LocalResource : IResource
    {
        public char PathSeparator { get; } = System.IO.Path.DirectorySeparatorChar;

        public string Path { get; set; }

        public string Name { get; set; }

        public bool Directory { get; set; }

        public long? Size { get; set; }

        public IClient Client { get; set; }

        public string CombinePath(string path)
        {
            return System.IO.Path.Combine(Path, path);
        }
    }

    public class LocalClient : IClient
    {
        public string RootPath { get; } = @"C:\";

        public event EventHandler<ClientProgress> OnProgressChange;

        private LocalResource MakeResource(string path)
        {
            var fileInfo = new FileInfo(path);
            var resource = new LocalResource() { Path = fileInfo.FullName, Name = fileInfo.Name, Client = this };

            if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                resource.Directory = true;
            }
            else
            {
                resource.Size = fileInfo.Length;
                resource.Directory = false;
            }

            return resource;
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task<IResource> Resource(string path)
        {
            return Task.FromResult<IResource>(MakeResource(path));
        }

        public Task<List<IResource>> Resources(string path)
        {
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            var items = directories.Concat(files);
            return Task.FromResult(items.Select(MakeResource).Cast<IResource>().ToList());
        }

        public async Task Delete(string path)
        {
            var resource = await Resource(path);

            if (resource.Directory)
            {
                foreach(var file in Directory.GetFiles(resource.Path))
                {
                    File.Delete(file);
                }

                foreach (var directory in Directory.GetDirectories(resource.Path))
                {
                    await Delete(directory);
                }

                Directory.Delete(resource.Path);
            }
            else
            {
                File.Delete(resource.Path);
            }
        }

        public Task DownloadDirectory(string source, string destination, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task DownloadFile(string source, string destination, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task UploadDirectory(string source, string destination, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(string source, string destination, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
