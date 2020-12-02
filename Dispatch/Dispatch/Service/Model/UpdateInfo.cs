using System;

namespace Dispatch.Service.Model
{
    public class UpdateInfo
    {
        public Version Version { get; set; }

        public string ReleaseNotes { get; set; }

        public long DownloadSize { get; set; }

        public string DownloadUrl { get; set; }
    }
}
