using Dispatch.Service.Model;
using System.Threading.Tasks;

namespace Dispatch.Service.Updater
{
    public interface IUpdateProvider
    {
        Task<Update> GetLatestUpdate();
    }
}
