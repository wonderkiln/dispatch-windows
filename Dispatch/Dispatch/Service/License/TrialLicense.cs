using System;

namespace Dispatch.Service.License
{
    public class TrialLicense : ILicense
    {
        public DateTime ExpiresAt { get; set; }

        public bool IsExpired
        {
            get
            {
                return DateTime.Now < ExpiresAt;
            }
        }
    }
}
