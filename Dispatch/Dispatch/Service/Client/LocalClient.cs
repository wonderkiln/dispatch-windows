using Dispatch.Service.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public class LocalClient : IClient
    {
        public static readonly string AllDrivesPathKey = "AllDrivesPathKey";

        public bool ShowHiddenFiles { get; set; } = false;

        public Task<IClient> Clone()
        {
            return Task.FromResult<IClient>(this);
        }

        public Task Diconnect()
        {
            return Task.CompletedTask;
        }

        private Resource MakeResource(string path)
        {
            if (path == AllDrivesPathKey)
            {
                return new Resource(this, AllDrivesPathKey, "All Drives");
            }
            else if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);

                if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    return null;
                }

                return new Resource(this, fileInfo.FullName, fileInfo.Name)
                {
                    Type = ResourceType.File,
                    Size = fileInfo.Length,
                    Modified = fileInfo.LastWriteTime,
                };
            }
            else if (Directory.Exists(path))
            {
                var directoryInfo = new DirectoryInfo(path);

                if (directoryInfo.Parent == null)
                {
                    var driveInfo = new DriveInfo(path);

                    var name = !string.IsNullOrEmpty(driveInfo.VolumeLabel) ?
                        $"{driveInfo.VolumeLabel} ({driveInfo.Name})" :
                        driveInfo.Name;

                    return new Resource(this, driveInfo.Name, name)
                    {
                        Type = ResourceType.Drive,
                        Size = driveInfo.TotalSize,
                    };
                }
                else
                {
                    if (directoryInfo.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        return null;
                    }

                    return new Resource(this, directoryInfo.FullName, directoryInfo.Name)
                    {
                        Type = ResourceType.Directory,
                    };
                }
            }
            else
            {
                throw new Exception($"File or directory not found at path: {path}");
            }
        }

        public Task<Resource> FetchResource(string path)
        {
            return Task.FromResult(MakeResource(path));
        }

        public Task<Resource[]> FetchResources(string path)
        {
            if (path == AllDrivesPathKey)
            {
                return Task.FromResult(Directory.GetLogicalDrives().Select(MakeResource).Where(e => e != null).ToArray());
            }
            else if (File.Exists(path))
            {
                var result = new Resource[] { MakeResource(path) };
                return Task.FromResult(result.Where(e => e != null).ToArray());
            }
            else if (Directory.Exists(path))
            {
                var directories = Directory.GetDirectories(path).Select(MakeResource).Where(e => e != null);
                var files = Directory.GetFiles(path).Select(MakeResource).Where(e => e != null);
                return Task.FromResult(directories.Concat(files).ToArray());
            }
            else
            {
                throw new Exception($"File or directory not found at path: {path}");
            }
        }

        public Task Delete(string path, CancellationToken token = default)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else
            {
                throw new Exception($"File or directory not found at path: {path}");
            }

            return Task.CompletedTask;
        }

        public Task Upload(string path, string fileOrDirectory, IProgress<ResourceProgress> progress = null, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task Download(string path, string toDirectory, IProgress<ResourceProgress> progress = null, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
