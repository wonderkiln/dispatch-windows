using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Client
{
    public interface IClient
    {
        string Root { get; }

        Task Disconnect();

        Task<List<Resource>> List(string path);
    }
}
