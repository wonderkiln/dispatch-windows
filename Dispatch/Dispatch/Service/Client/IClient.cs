using Dispatch.Service.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public interface IClient
    {
        Task<IClient> Clone();

        Task Diconnect();

        Task<Resource> FetchResource(string path);

        Task<Resource[]> FetchResources(string path);

        Task Delete(string path, CancellationToken token = default);

        Task Upload(string path, string fileOrDirectory, IProgress<ResourceProgress> progress = null, CancellationToken token = default);

        Task Download(string path, string toDirectory, IProgress<ResourceProgress> progress = null, CancellationToken token = default);
    }
}
