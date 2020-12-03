using Dispatch.Service.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public interface IClient
    {
        string Name { get; }

        string InitialPath { get; }

        Task<IClient> Clone();

        Task Diconnect();

        Task<Resource> FetchResource(string path);

        Task<Resource[]> FetchResources(string path);

        Task Delete(string path, CancellationToken token = default);

        Task Upload(string path, string fileOrDirectory, IProgress<ProgressStatus> progress = null, CancellationToken token = default);
    }
}
