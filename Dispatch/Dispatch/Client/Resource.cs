using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Client
{
    public enum ResourceType
    {
        File, Link, Directory
    }

    public class Resource
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public ResourceType Type { get; set; }

        public long? Size { get; set; }

        public IClient Client { get; set; }
    }
}
