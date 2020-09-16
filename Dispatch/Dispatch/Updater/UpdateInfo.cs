using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Updater
{
    public class UpdateInfo
    {
        public Version Version { get; set; }

        public string ReleaseNotes { get; set; }

        public long DownloadSize { get; set; }

        public string DownloadUrl { get; set; }
    }
}
