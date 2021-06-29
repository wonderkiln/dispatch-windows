using Dispatch.Service.Models;
using System.Threading.Tasks;

namespace Dispatch.Service.Updater
{
    public interface IUpdateProvider
    {
        Task<Update> GetLatestUpdate();
    }
}
