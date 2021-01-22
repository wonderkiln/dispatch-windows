using Dispatch.Service.Model;
using Renci.SshNet;
using Renci.SshNet.Common;
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
            client.HostKeyReceived += Client_HostKeyReceived;

            return Task.Run(() =>
            {
                client.Connect();
                return new SFTPClient(client);
            });
        }

        private static void Client_HostKeyReceived(object sender, HostKeyEventArgs e)
        {
            e.CanTrust = true;
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

        public async Task Delete(string path, CancellationToken token = default)
        {
            var resource = await FetchResource(path);

            if (resource.Type == ResourceType.File)
            {
                Client.Delete(path);
            }
            else
            {
                var resources = await FetchResources(path);

                foreach (var item in resources)
                {
                    token.ThrowIfCancellationRequested();
                    await Delete(item.Path, token);
                }

                Client.Delete(path);
            }
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

                using (var stream = new StreamReader(fileOrDirectory))
                {
                    // Forcefully close the file stream when cancelled so the upload throws an error
                    // TODO: Dispose
                    token.Register(() =>
                    {
                        stream.Close();
                    });

                    Client.UploadFile(stream.BaseStream, destination, (length) =>
                    {
                        var size = new FileInfo(fileOrDirectory).Length;
                        var value = 100 * length / (double)size;
                        progress?.Report(new ProgressStatus(0, 1, value));
                    });
                }
            }
            else if (Directory.Exists(fileOrDirectory))
            {
                var destinationDirectory = $"{normalizedPath}/{Path.GetFileName(fileOrDirectory)}";

                if (!Client.Exists(destinationDirectory))
                {
                    Client.CreateDirectory(destinationDirectory);
                }

                var totalFiles = Directory.GetFiles(fileOrDirectory, "*", SearchOption.AllDirectories).Length;

                var directories = Directory.GetDirectories(fileOrDirectory);
                var files = Directory.GetFiles(fileOrDirectory);

                var newProgress = progress is SingleToMultiFileProgress ? (SingleToMultiFileProgress)progress : new SingleToMultiFileProgress(progress, 0, totalFiles);

                foreach (var file in files)
                {
                    token.ThrowIfCancellationRequested();
                    await Upload(destinationDirectory, file, newProgress, token);
                    newProgress.IncrementIndex();
                }
                foreach (var directory in directories)
                {
                    token.ThrowIfCancellationRequested();
                    await Upload(destinationDirectory, directory, newProgress, token);
                }
            }
            else
            {
                throw new Exception($"File or directory not found at path: {fileOrDirectory}");
            }
        }
    }

    internal class SingleToMultiFileProgress : IProgress<ProgressStatus>
    {
        IProgress<ProgressStatus> progress;
        int index;
        readonly int count;

        public SingleToMultiFileProgress(IProgress<ProgressStatus> progress, int index, int count)
        {
            this.progress = progress;
            this.index = index;
            this.count = count;
        }

        public void IncrementIndex()
        {
            index += 1;
        }

        public void Report(ProgressStatus value)
        {
            progress?.Report(new ProgressStatus(index, count, value.Progress));
        }
    }
}
