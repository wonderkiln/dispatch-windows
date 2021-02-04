using Dispatch.Service.Models;
using System;

namespace Dispatch.Service.Licensing
{
    public class LicenseApi
    {
        public string HardwareIdentifier { get; }

        public LicenseApi(string hardwareIdentifier)
        {
            HardwareIdentifier = hardwareIdentifier;
        }

        public License VerifyLicense(License license)
        {
            return license;
        }

        public License GetTrialLicense()
        {
            return new License() { Id = "_trial", Expiration = DateTime.Now.AddDays(30), IsTrial = true };
        }

        public License GetPaidLicense(string email, string key)
        {
            return new License() { Id = "_paid", Expiration = DateTime.Now.AddDays(30), MajorVersion = 0 };
        }

        public void RemoveLicense(License license)
        {
            //
        }
    }
}
