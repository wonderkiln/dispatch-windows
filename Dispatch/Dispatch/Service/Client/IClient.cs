using Dispatch.Service.Model;
using System.Threading.Tasks;

namespace Dispatch.Service.Client
{
    public interface IClient
    {
        string InitialPath { get; }

        Task Diconnect();

        Task<Resource> FetchResource(string path);

        Task<Resource[]> FetchResources(string path);
    }
}
