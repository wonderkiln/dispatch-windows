using Dispatch.Service.Model;
using System;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public interface IClient
    {
        string Name { get; }

        string InitialPath { get; }

        Task Diconnect();

        Task<Resource> FetchResource(string path);

        Task<Resource[]> FetchResources(string path);

        Task Delete(string path);

        Task Upload(string path, string fileOrDirectory, IProgress<double> progress = null);
    }
}
