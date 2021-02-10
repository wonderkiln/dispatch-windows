using Dispatch.Helpers;
using System;

namespace Dispatch.Service.Models
{
    public class License
    {
        public string Id { get; set; }

        public DateTime Expiration { get; set; }

        public int MajorVersion { get; set; }

        public bool IsTrial { get; set; }

        public bool IsExpired
        {
            get
            {
                return DateTime.Now > Expiration;
            }
        }

        public bool IsSameMajorVersion
        {
            get
            {
                return Constants.VERSION.Major == MajorVersion;
            }
        }
    }
}
