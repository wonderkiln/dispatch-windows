using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Updater
{
    public interface IUpdateProvider
    {
        event EventHandler<double> DownloadProgressChanged;

        Task<UpdateInfo> GetLatestUpdate();

        Task<string> DownloadUpdate(UpdateInfo info);
    }
}
