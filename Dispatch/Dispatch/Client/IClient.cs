using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatch.Client
{
    public class ClientProgress
    {
        public string RemotePath { get; set; }

        public string LocalPath { get; set; }

        public double TotalProgress { get; set; }
    }

    public interface IClient
    {
        event EventHandler<ClientProgress> OnProgressChange;

        string RootPath { get; }

        Task Disconnect();

        Task<IResource> Resource(string path);

        Task<List<IResource>> Resources(string path);

        Task DownloadFile(string source, string destination);

        Task DownloadDirectory(string source, string destination);

        Task UploadFile(string source, string destination);

        Task UploadDirectory(string source, string destination);
    }
}
