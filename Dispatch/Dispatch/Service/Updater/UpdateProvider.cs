using Dispatch.Service.API;
using Dispatch.Service.Models;
using System.Threading.Tasks;

namespace Dispatch.Service.Updater
{
    public class UpdateProvider : IUpdateProvider
    {
        private readonly APIClient client = new APIClient(null);

        public Task<Update> GetLatestUpdate()
        {
            return client.GetLatestUpdate();
        }
    }
}
