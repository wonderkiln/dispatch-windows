using Dispatch.Service.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public class LocalClient : IClient
    {
        public static readonly string AllDrivesPathKey = "";

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

        public Task Upload(string path, string fileOrDirectory, IProgress<ProgressStatus> progress = null, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task Download(string path, string toDirectory, IProgress<ProgressStatus> progress = null, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
