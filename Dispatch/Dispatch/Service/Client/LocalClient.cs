using Dispatch.Service.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public class LocalClient : IClient
    {
        private static readonly string AllDrivesPathKey = "_AllDrivesPathKey";

        public string InitialPath { get; } = AllDrivesPathKey;

        public Task Diconnect()
        {
            return Task.CompletedTask;
        }

        private Resource MakeResource(string path)
        {
            if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                return new Resource(this, fileInfo.FullName, fileInfo.Name)
                {
                    Type = ResourceType.File,
                    Size = fileInfo.Length,
                };
            }
            else if (Directory.Exists(path))
            {
                var directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Parent == null)
                {
                    var driveInfo = new DriveInfo(path);
                    return new Resource(this, driveInfo.Name, !string.IsNullOrEmpty(driveInfo.VolumeLabel) ? driveInfo.VolumeLabel : driveInfo.Name)
                    {
                        Type = ResourceType.Drive,
                        Size = driveInfo.TotalSize,
                    };
                }
                else
                {
                    return new Resource(this, directoryInfo.FullName, directoryInfo.Name)
                    {
                        Type = ResourceType.Directory,
                    };
                }
            }
            else
            {
                throw new Exception("File or directory not found");
            }
        }

        public Task<Resource> FetchResource(string path)
        {
            if (path == AllDrivesPathKey)
            {
                return Task.FromResult<Resource>(null);
            }

            return Task.FromResult(MakeResource(path));
        }

        public Task<Resource[]> FetchResources(string path)
        {
            if (path == AllDrivesPathKey)
            {
                return Task.FromResult(Directory.GetLogicalDrives().Select(MakeResource).ToArray());
            }
            else if (File.Exists(path))
            {
                return Task.FromResult(new Resource[] { MakeResource(path) });
            }
            else if (Directory.Exists(path))
            {
                var directories = Directory.GetDirectories(path).Select(MakeResource);
                var files = Directory.GetFiles(path).Select(MakeResource);
                return Task.FromResult(directories.Concat(files).ToArray());
            }
            else
            {
                throw new Exception("File or directory not found");
            }
        }
    }
}
