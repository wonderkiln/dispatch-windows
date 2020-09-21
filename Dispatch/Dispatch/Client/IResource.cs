using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Client
{
    public interface IResource
    {
        string Path { get; }

        string Name { get; }

        bool Directory { get; }

        long? Size { get; }

        IClient Client { get; }

        string CombinePath(string path);
    }
}
