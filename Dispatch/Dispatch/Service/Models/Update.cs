using System;

namespace Dispatch.Service.Models
{
    public class Update
    {
        public Version Version { get; }

        public string Notes { get; }

        public long Size { get; }

        public Uri Link { get; }

        public Update(Version version, string notes, long size, Uri link)
        {
            Version = version;
            Notes = notes;
            Size = size;
            Link = link;
        }
    }
}
