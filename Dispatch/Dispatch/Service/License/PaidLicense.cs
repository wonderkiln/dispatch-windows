using Dispatch.Helpers;
using System;

namespace Dispatch.Service.License
{
    public class PaidLicense : ILicense
    {
        public DateTime ExpiresAt { get; set; }
        public int MajorVersion { get; set; }

        public bool IsExpired
        {
            get
            {
                return DateTime.Now < ExpiresAt;
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
