using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Dispatch.Service.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public class S3Client : IClient
    {
        private readonly AmazonS3Client client;

        private readonly S3Connection connectionInfo;

        public S3Client(AmazonS3Client client, S3Connection connectionInfo)
        {
            this.client = client;
            this.connectionInfo = connectionInfo;
        }

        public static Task<S3Client> Create(S3Connection connectionInfo)
        {
            var config = new AmazonS3Config() { ServiceURL = connectionInfo.Server };
            var client = new AmazonS3Client(connectionInfo.Key, connectionInfo.Secret, config);

            return Task.FromResult(new S3Client(client, connectionInfo));
        }

        public async Task<IClient> Clone()
        {
            return await Create(connectionInfo);
        }

        public Task Diconnect()
        {
            client.Dispose();
            return Task.CompletedTask;
        }

        private Resource MakeResource(S3Object item)
        {
            var path = "/" + item.BucketName + "/" + item.Key;
            var parsedPath = new ParsedPath(path);

            if (item.Key.EndsWith("/") && item.Size == 0)
            {
                return new Resource(this, parsedPath.Path, parsedPath.Name)
                {
                    Type = ResourceType.Directory,
                };
            }
            else
            {
                return new Resource(this, parsedPath.Path, parsedPath.Name)
                {
                    Size = item.Size,
                    Type = ResourceType.File,
                    Modified = item.LastModified,
                };
            }


        }

        private Resource MakeResource(S3Bucket item)
        {
            return new Resource(this, "/" + item.BucketName, item.BucketName)
            {
                Type = ResourceType.Directory,
            };
        }

        private class ParsedPath
        {
            public string Bucket { get; set; }

            public string Key { get; set; }

            public string Name { get; set; }

            public bool HasBucket
            {
                get
                {
                    return !string.IsNullOrEmpty(Bucket);
                }
            }

            public bool HasKey
            {
                get
                {
                    return !string.IsNullOrEmpty(Key);
                }
            }

            public string Path
            {
                get
                {
                    if (!HasBucket) return "/";
                    if (!HasKey) return "/" + Bucket;
                    return "/" + Bucket + "/" + Key;
                }
            }

            public ParsedPath(string path)
            {
                var components = path.Split('/').Where(e => !string.IsNullOrEmpty(e)).ToList();

                if (components.Count > 0)
                {
                    Bucket = components[0];
                }

                if (components.Count > 1)
                {
                    Key = components.Skip(1).ToArray().Join("/");
                    Name = components.Last().Replace("/", "");
                }
            }
        }


        public async Task<Resource> FetchResource(string path)
        {
            var parsedPath = new ParsedPath(path);

            if (!parsedPath.HasBucket)
            {
                return new Resource(this, path, "All Buckets");
            }
            else if (!parsedPath.HasKey)
            {
                var buckets = await client.ListBucketsAsync();
                var bucket = buckets.Buckets.Find(e => e.BucketName == parsedPath.Bucket);
                return MakeResource(bucket);
            }

            var request = new ListObjectsV2Request() { BucketName = parsedPath.Bucket, Prefix = parsedPath.Key };
            var objects = await client.ListObjectsV2Async(request);

            var foundObject = objects.S3Objects.FirstOrDefault(e => e.Key == path);
            if (foundObject != null) return MakeResource(foundObject);

            return new Resource(this, path, parsedPath.Name)
            {
                Type = ResourceType.Directory
            };
        }

        public async Task<Resource[]> FetchResources(string path)
        {
            var parsedPath = new ParsedPath(path);

            if (!parsedPath.HasBucket)
            {
                var buckets = await client.ListBucketsAsync();
                return buckets.Buckets.Select(MakeResource).ToArray();
            }

            var request = new ListObjectsV2Request() { BucketName = parsedPath.Bucket, Prefix = parsedPath.Key };
            var objects = await client.ListObjectsV2Async(request);

            var resources = new List<Resource>();

            foreach (var item in objects.S3Objects)
            {
                var relativeKey = parsedPath.HasKey ? item.Key.Replace(parsedPath.Key, "") : item.Key;
                var components = relativeKey.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();

                if (components.Length == 1)
                {
                    resources.Add(MakeResource(item));
                }
                else if (components.Length > 1)
                {
                    var itemPath = parsedPath.Path + "/" + components[0];
                    var resource = new Resource(this, itemPath, components[0]) { Type = ResourceType.Directory };
                    if (!resources.Contains(resource)) resources.Add(resource);
                }
            }

            return resources.ToArray();
        }

        public async Task Delete(string path, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var parsedPath = new ParsedPath(path);
            var resource = await FetchResource(path);

            if (parsedPath.HasKey)
            {
                if (resource.Type == ResourceType.File)
                {
                    await client.DeleteObjectAsync(parsedPath.Bucket, parsedPath.Key, token);
                }
                else
                {
                    var objects = await client.ListObjectsV2Async(new ListObjectsV2Request()
                    {
                        Prefix = parsedPath.Key,
                        BucketName = parsedPath.Bucket,
                    });

                    foreach (var item in objects.S3Objects)
                    {
                        await client.DeleteObjectAsync(item.BucketName, item.Key, token);
                    }
                }
            }
            else
            {
                throw new Exception("Can only delete objects");
            }
        }

        private class S3ProgressConverter
        {
            private readonly IProgress<ResourceProgress> progress;
            private readonly int count;

            private int index = 0;

            public string Path { get; set; }

            public S3ProgressConverter(IProgress<ResourceProgress> progress, int count = 1)
            {
                this.progress = progress;
                this.count = count;
            }

            public void StreamTransferProgress(object sender, StreamTransferProgressArgs e)
            {
                progress.Report(new ResourceProgress(index, count, e.PercentDone, Path));
            }

            public void WriteObjectProgress(object sender, WriteObjectProgressArgs e)
            {
                progress.Report(new ResourceProgress(index, count, e.PercentDone, Path));
            }

            public void Next()
            {
                index += 1;
            }
        }

        public async Task Upload(string path, string fileOrDirectory, IProgress<ResourceProgress> progress = null, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var parsedPath = new ParsedPath(path);

            if (parsedPath.HasBucket)
            {
                if (File.Exists(fileOrDirectory))
                {
                    var progressConverter = new S3ProgressConverter(progress);
                    var request = new UploadPartRequest()
                    {
                        FilePath = fileOrDirectory,
                        BucketName = parsedPath.Bucket,
                        StreamTransferProgress = progressConverter.StreamTransferProgress,
                        Key = (parsedPath.HasKey ? parsedPath.Key + "/" : "") + Path.GetFileName(fileOrDirectory),
                    };

                    await client.UploadPartAsync(request, token);
                }
                else if (Directory.Exists(fileOrDirectory))
                {
                    var name = Path.GetFileName(fileOrDirectory);
                    var files = Directory.GetFiles(fileOrDirectory, "*", SearchOption.AllDirectories);
                    var progressConverter = new S3ProgressConverter(progress, files.Length);

                    foreach (var file in files)
                    {
                        progressConverter.Path = file;

                        var newPath = file
                            .Substring(fileOrDirectory.Length)
                            .Split(Path.DirectorySeparatorChar)
                            .Where(e => !string.IsNullOrEmpty(e))
                            .ToArray()
                            .Join("/");

                        var request = new PutObjectRequest()
                        {
                            FilePath = file,
                            BucketName = parsedPath.Bucket,
                            StreamTransferProgress = progressConverter.StreamTransferProgress,
                            Key = (parsedPath.HasKey ? parsedPath.Key + "/" : "") + name + "/" + newPath,
                        };

                        await client.PutObjectAsync(request, token);

                        progressConverter.Next();
                    }
                }
                else
                {
                    throw new Exception($"File or directory not found at path: {fileOrDirectory}");
                }
            }
            else
            {
                throw new Exception("Can only upload to buckets and objects");
            }
        }

        public async Task Download(string path, string toDirectory, IProgress<ResourceProgress> progress = null, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var parsedPath = new ParsedPath(path);
            var resource = await FetchResource(path);

            if (parsedPath.HasKey)
            {
                if (resource.Type == ResourceType.File)
                {
                    var progressConverter = new S3ProgressConverter(progress);
                    var request = new GetObjectRequest()
                    {
                        Key = parsedPath.Key,
                        BucketName = parsedPath.Bucket,
                    };

                    var response = await client.GetObjectAsync(request, token);

                    response.WriteObjectProgressEvent += progressConverter.WriteObjectProgress;

                    await response.WriteResponseStreamToFileAsync(Path.Combine(toDirectory, parsedPath.Name), false, token);
                }
                else if (resource.Type == ResourceType.Directory)
                {
                    var objects = await client.ListObjectsV2Async(new ListObjectsV2Request()
                    {
                        Prefix = parsedPath.Key,
                        BucketName = parsedPath.Bucket,
                    });

                    var items = objects.S3Objects.Where(e => e.Size > 0).ToArray();

                    var progressConverter = new S3ProgressConverter(progress, items.Length);

                    foreach (var item in items)
                    {
                        progressConverter.Path = item.Key;

                        var response = await client.GetObjectAsync(new GetObjectRequest()
                        {
                            Key = item.Key,
                            BucketName = item.BucketName,
                        }, token);

                        response.WriteObjectProgressEvent += progressConverter.WriteObjectProgress;

                        var newKey = item.Key.Replace(parsedPath.Key, parsedPath.Name).Split('/').Where(e => !string.IsNullOrEmpty(e)).ToArray().Join(Path.DirectorySeparatorChar.ToString());
                        await response.WriteResponseStreamToFileAsync(Path.Combine(toDirectory, newKey), false, token);

                        progressConverter.Next();
                    }
                }
            }
            else
            {
                throw new Exception("Can only download from objects");
            }
        }
    }
}
