using Dispatch.Service.Model;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public class SFTPClient : IClient
    {
        private readonly SftpClient Client;

        public SFTPClient(SftpClient client)
        {
            Client = client;
        }

        private static Task<SFTPClient> Create(ConnectionInfo connectionInfo)
        {
            var client = new SftpClient(connectionInfo);

            return Task.Run(() =>
            {
                client.Connect();
                return new SFTPClient(client);
            });
        }

        public static Task<SFTPClient> Create(string host, int port, string username, string password)
        {
            var connectionInfo = new ConnectionInfo(host, port, username, new PasswordAuthenticationMethod(username, password));
            return Create(connectionInfo);
        }

        public async Task<IClient> Clone()
        {
            return await Create(Client.ConnectionInfo);
        }

        public Task Delete(string path, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                Client.Delete(path);
            },
            token);
        }

        public Task Diconnect()
        {
            Client.Disconnect();
            Client.Dispose();

            return Task.CompletedTask;
        }

        private async Task<Dictionary<string, Resource>> MapResourcesToLocal(string path, string toDirectory)
        {
            var result = new Dictionary<string, Resource>();

            var resources = await FetchResources(path);

            var files = resources.Where(e => e.Type != ResourceType.Directory);

            foreach (var file in files)
            {
                var localPath = Path.Combine(toDirectory, file.Name);
                result.Add(localPath, file);
            }

            var directories = resources.Where(e => e.Type == ResourceType.Directory);

            foreach (var directory in directories)
            {
                var localPath = Path.Combine(toDirectory, directory.Name);
                var subFiles = await MapResourcesToLocal(directory.Path, localPath);

                foreach (var item in subFiles)
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return result;
        }

        public async Task Download(string path, string toDirectory, IProgress<ProgressStatus> progress = null, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var resource = await FetchResource(path);

            var localPath = Path.Combine(toDirectory, resource.Name);

            if (resource.Type == ResourceType.File)
            {
                await Task.Run(() =>
                {
                    using (var stream = new StreamWriter(localPath))
                    {
                        Client.DownloadFile(path, stream.BaseStream, (length) =>
                        {
                            var value = 100 * length / (double)resource.Size;
                            progress?.Report(new ProgressStatus(0, 1, value));
                        });
                    }
                },
                token);
            }
            else if (resource.Type == ResourceType.Directory)
            {
                var items = await MapResourcesToLocal(path, localPath);
                var index = 0;

                foreach (var item in items)
                {
                    await Task.Run(() =>
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(item.Key));

                        using (var stream = new StreamWriter(item.Key))
                        {
                            Client.DownloadFile(item.Value.Path, stream.BaseStream, (length) =>
                            {
                                var value = 100 * length / (double)item.Value.Size;
                                progress?.Report(new ProgressStatus(index, items.Count, value));
                            });
                        }
                    },
                    token);

                    index += 1;
                }
            }
        }

        public Task<Resource> FetchResource(string path)
        {
            return Task.Run(() =>
            {
                return MakeResource(Client.Get(path));
            });
        }

        private Resource MakeResource(SftpFile item)
        {
            if (item.IsDirectory)
            {
                return new Resource(this, item.FullName, item.Name)
                {
                    Type = ResourceType.Directory,
                };
            }

            return new Resource(this, item.FullName, item.Name)
            {
                Type = ResourceType.File,
                Size = item.Length,
            };
        }

        private static readonly string[] HIDDEN_FILE_NAMES = new string[] { ".", ".." };

        public Task<Resource[]> FetchResources(string path)
        {
            return Task.Run(() =>
            {
                var items = Client.ListDirectory(path).Where(e => !HIDDEN_FILE_NAMES.Contains(e.Name));
                return items.Select(MakeResource).ToArray();
            });
        }

        public async Task Upload(string path, string fileOrDirectory, IProgress<ProgressStatus> progress = null, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var normalizedPath = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;

            if (File.Exists(fileOrDirectory))
            {
                var destination = $"{normalizedPath}/{Path.GetFileName(fileOrDirectory)}";

                await Task.Run(() =>
                {
                    using (var stream = new StreamReader(fileOrDirectory))
                    {
                        Client.UploadFile(stream.BaseStream, destination, (length) =>
                        {
                            var size = new FileInfo(fileOrDirectory).Length;
                            var value = 100 * length / (double)size;
                            progress?.Report(new ProgressStatus(0, 1, value));
                        });
                    }
                });
            }
            else if (Directory.Exists(fileOrDirectory))
            {
                // TODO: get all the files
                var destination = $"{normalizedPath}/{Path.GetFileName(fileOrDirectory)}";

                var files = Directory.GetFiles(fileOrDirectory);
                
                var directories = Directory.GetDirectories(fileOrDirectory);

                // D:/x/xx/xxx.txt -> {path}/x/xx/xxx.txt
            }
            else
            {
                throw new Exception($"File or directory not found at path: {fileOrDirectory}");
            }
        }
    }
}
