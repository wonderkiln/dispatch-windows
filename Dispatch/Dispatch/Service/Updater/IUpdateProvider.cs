using Dispatch.Service.Model;
using System;
using System.Threading.Tasks;

namespace Dispatch.Service.Updater
{
    public interface IUpdateProvider
    {
        event EventHandler<double> DownloadProgressChanged;

        Task<UpdateInfo> GetLatestUpdate();

        Task<string> DownloadUpdate(UpdateInfo info);
    }
}
