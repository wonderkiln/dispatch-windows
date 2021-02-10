using Dispatch.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dispatch.Service.Licensing
{
    public class LicenseManager
    {
        public enum Status { None, Trial, TrialExpired, Paid, PaidExpired, PaidExpiredWrongVersion }

        public static LicenseManager Shared = new LicenseManager();

        private readonly LicenseWriter writer = new LicenseWriter();

        // Save locally to be faster?
        private readonly LicenseApi api = new LicenseApi(Fingerprint.GetFingerprint());

        public LicenseManager()
        {
            Console.WriteLine("License manager initialized with hardware identifier: {0}", api.HardwareIdentifier);
        }

        // Get locally available installed licenses
        private License[] GetInstalledLicenses()
        {
            return writer.GetAllLicenses();
        }

        // Verify and validate the licenses with the API and update them locally.
        // On error remove that license locally.
        private License[] RefreshLicenses(License[] licenses)
        {
            var newLicenses = new List<License>();

            foreach (var license in licenses)
            {
                var newLicense = api.VerifyLicense(license);
                writer.ReplaceLicense(license, newLicense);
                newLicenses.Add(newLicense);
            }

            return newLicenses.ToArray();
        }

        public void InstallTrialLicense()
        {
            // TODO: no more trials
            var license = api.GetTrialLicense();
            writer.AddLicense(license);
        }

        public void InstallPaidLicense(string email, string key)
        {
            // TODO: no more paid
            var license = api.GetPaidLicense(email, key);
            writer.AddLicense(license);
        }

        // Returns the status of the licensing, validating them with the API and locally
        public Status GetLicenseStatus()
        {
            var installedLicenses = GetInstalledLicenses();
            var refreshedLicenses = RefreshLicenses(installedLicenses);

            var paidLicenses = refreshedLicenses.Where(e => !e.IsTrial);
            var trialLicenses = refreshedLicenses.Where(e => e.IsTrial);

            foreach (var license in paidLicenses)
            {
                if (!license.IsExpired)
                {
                    return Status.Paid;
                }
                else if (license.IsSameMajorVersion)
                {
                    return Status.PaidExpired;
                }

                return Status.PaidExpiredWrongVersion;
            }

            foreach (var license in trialLicenses)
            {
                if (!license.IsExpired)
                {
                    return Status.Trial;
                }

                return Status.TrialExpired;
            }

            return Status.None;
        }

        // Remove the local (instant) + remote paid license.
        // Does not remove the trial ones.
        // Need to re-calculate the status.
        public void RemoveLicense(License license)
        {
            writer.RemoveLicense(license);
            api.RemoveLicense(license);
        }
    }
}
