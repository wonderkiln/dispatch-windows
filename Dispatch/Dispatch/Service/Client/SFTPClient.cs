using Dispatch.Service.Models;
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
        private readonly SftpClient client;

        private readonly SFTPConnection connectionInfo;

        public SFTPClient(SftpClient client, SFTPConnection connectionInfo)
        {
            this.client = client;
            this.connectionInfo = connectionInfo;
        }

        public static Task<SFTPClient> Create(SFTPConnection connectionInfo)
        {
            ConnectionInfo info;

            if (!string.IsNullOrEmpty(connectionInfo.Key))
            {
                var authMethod = new PrivateKeyAuthenticationMethod(connectionInfo.Username, new PrivateKeyFile(connectionInfo.Key));
                info = new ConnectionInfo(connectionInfo.Address, connectionInfo.Port, connectionInfo.Username, authMethod);
            }
            else
            {
                var authMethod1 = new PasswordAuthenticationMethod(connectionInfo.Username, connectionInfo.Password);
                var authMethod2 = new KeyboardInteractiveAuthenticationMethod(connectionInfo.Username);

                authMethod2.AuthenticationPrompt += (sender, e) =>
                {
                    foreach (var prompt in e.Prompts)
                    {
                        prompt.Response = connectionInfo.Password;
                    }
                };

                info = new ConnectionInfo(connectionInfo.Address, connectionInfo.Port, connectionInfo.Username, authMethod1, authMethod2);
            }

            var client = new SftpClient(info);
            client.HostKeyReceived += Client_HostKeyReceived;

            return Task.Run(() =>
            {
                client.Connect();
                return new SFTPClient(client, connectionInfo);
            });
        }

        private static void Client_HostKeyReceived(object sender, HostKeyEventArgs e)
        {
            e.CanTrust = true;
        }

        public async Task<IClient> Clone()
        {
            return await Create(connectionInfo);
        }

        public async Task Delete(string path, CancellationToken token = default)
        {
            var resource = await FetchResource(path);

            if (resource.Type == ResourceType.File)
            {
                client.Delete(path);
            }
            else
            {
                var resources = await FetchResources(path);

                foreach (var item in resources)
                {
                    token.ThrowIfCancellationRequested();
                    await Delete(item.Path, token);
                }

                client.Delete(path);
            }
        }

        public Task Diconnect()
        {
            client.Disconnect();
            client.Dispose();

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

        public async Task Download(string path, string toDirectory, IProgress<ResourceProgress> progress = null, CancellationToken token = default)
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
                        client.DownloadFile(path, stream.BaseStream, (length) =>
                        {
                            var value = 100 * length / (double)resource.Size;
                            progress?.Report(new ResourceProgress(0, 1, value));
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
                            client.DownloadFile(item.Value.Path, stream.BaseStream, (length) =>
                            {
                                var value = 100 * length / (double)item.Value.Size;
                                progress?.Report(new ResourceProgress(index, items.Count, value));
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
                return MakeResource(client.Get(path));
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
                var items = client.ListDirectory(path).Where(e => !HIDDEN_FILE_NAMES.Contains(e.Name));
                return items.Select(MakeResource).ToArray();
            });
        }

        public async Task Upload(string path, string fileOrDirectory, IProgress<ResourceProgress> progress = null, CancellationToken token = default)
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

                    client.UploadFile(stream.BaseStream, destination, (length) =>
                    {
                        var size = new FileInfo(fileOrDirectory).Length;
                        var value = 100 * length / (double)size;
                        progress?.Report(new ResourceProgress(0, 1, value));
                    });
                }
            }
            else if (Directory.Exists(fileOrDirectory))
            {
                var destinationDirectory = $"{normalizedPath}/{Path.GetFileName(fileOrDirectory)}";

                if (!client.Exists(destinationDirectory))
                {
                    client.CreateDirectory(destinationDirectory);
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

    internal class SingleToMultiFileProgress : IProgress<ResourceProgress>
    {
        private readonly IProgress<ResourceProgress> progress;
        private readonly int count;
        private int index;

        public SingleToMultiFileProgress(IProgress<ResourceProgress> progress, int index, int count)
        {
            this.progress = progress;
            this.index = index;
            this.count = count;
        }

        public void IncrementIndex()
        {
            index += 1;
        }

        public void Report(ResourceProgress value)
        {
            progress?.Report(new ResourceProgress(index, count, value.Progress));
        }
    }
}
