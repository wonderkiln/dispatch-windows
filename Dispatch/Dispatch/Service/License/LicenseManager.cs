using System;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Dispatch.Service.License
{
    public class FingerPrint
    {
        private static string fingerprint = string.Empty;

        public static string GetFingerprint()
        {
            if (string.IsNullOrEmpty(fingerprint))
            {
                fingerprint = GetHash("CPU >> " + CpuId() + "\nBIOS >> " + BiosId() + "\nBASE >> " + BaseId() + "\nDISK >> " + DiskId());
            }

            return fingerprint;
        }

        private static string GetHash(string value)
        {
            var provider = new MD5CryptoServiceProvider();
            var encoding = new ASCIIEncoding();
            var bytes = encoding.GetBytes(value);
            var hash = provider.ComputeHash(bytes);
            return BitConverter.ToString(hash);
        }

        private static string GetIdentifier(string path, string property)
        {
            var mc = new ManagementClass(path);
            var moc = mc.GetInstances();

            foreach (var mo in moc)
            {
                try
                {
                    var value = mo[property];

                    if (value != null)
                    {
                        return value.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
            }

            return string.Empty;
        }

        private static string CpuId()
        {
            // Uses first CPU identifier available in order of preference
            // Don't get all identifiers, as it is very time consuming
            var retVal = GetIdentifier("Win32_Processor", "UniqueId");
            if (string.IsNullOrEmpty(retVal)) // If no UniqueID, use ProcessorID
            {
                retVal = GetIdentifier("Win32_Processor", "ProcessorId");
                if (string.IsNullOrEmpty(retVal)) // If no ProcessorId, use Name
                {
                    retVal = GetIdentifier("Win32_Processor", "Name");
                    if (string.IsNullOrEmpty(retVal)) // If no Name, use Manufacturer
                    {
                        retVal = GetIdentifier("Win32_Processor", "Manufacturer");
                    }
                    // Add clock speed for extra security
                    retVal += GetIdentifier("Win32_Processor", "MaxClockSpeed");
                }
            }

            return retVal;
        }

        private static string BiosId()
        {
            return GetIdentifier("Win32_BIOS", "Manufacturer") + ";"
                + GetIdentifier("Win32_BIOS", "SMBIOSBIOSVersion") + ";"
                + GetIdentifier("Win32_BIOS", "IdentificationCode") + ";"
                + GetIdentifier("Win32_BIOS", "SerialNumber") + ";"
                + GetIdentifier("Win32_BIOS", "ReleaseDate") + ";"
                + GetIdentifier("Win32_BIOS", "Version");
        }

        private static string DiskId()
        {
            return GetIdentifier("Win32_DiskDrive", "Model") + ";"
                + GetIdentifier("Win32_DiskDrive", "Manufacturer") + ";"
                + GetIdentifier("Win32_DiskDrive", "Signature") + ";"
                + GetIdentifier("Win32_DiskDrive", "TotalHeads");
        }

        private static string BaseId()
        {
            return GetIdentifier("Win32_BaseBoard", "Model") + ";"
                + GetIdentifier("Win32_BaseBoard", "Manufacturer") + ";"
                + GetIdentifier("Win32_BaseBoard", "Name") + ";"
                + GetIdentifier("Win32_BaseBoard", "SerialNumber");
        }
    }

    public class LicenseManager
    {
        public enum Status { None, Trial, TrialExpired, Paid, PaidExpired, PaidExpiredWrongVersion }

        public static LicenseManager Shared = new LicenseManager();

        // Save locally to be faster?
        private readonly string HardwareIdentifier = FingerPrint.GetFingerprint();

        public LicenseManager()
        {
            Console.WriteLine("License manager initialized with hardware identifier: {0}", HardwareIdentifier);
        }

        // Get locally available installed licenses
        private ILicense[] GetInstalledLicenses()
        {
            return null;
        }

        // Verify and validate the licenses with the API and update them locally.
        // On error remove that license locally.
        private ILicense[] RefreshLicenses(ILicense[] licenses)
        {
            return null;
        }

        // Install a license making sure that type doesn't exist locally.
        // Need to re-calculate the status.
        public void InstallLicense(ILicense license)
        {
            InstallLicenseLocally(license);
        }

        // Returns the status of the licensing, validating them with the API and locally
        public Status GetLicenseStatus()
        {
            var installedLicenses = GetInstalledLicenses();
            var refreshedLicenses = RefreshLicenses(installedLicenses);

            var paidLicenses = refreshedLicenses.Where(e => e is PaidLicense).Cast<PaidLicense>();
            var trialLicenses = refreshedLicenses.Where(e => e is TrialLicense).Cast<TrialLicense>();

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
        public void RemoveLicense(ILicense license)
        {
            RemoveLicenseLocally(license);
        }

        private void InstallLicenseLocally(ILicense license)
        {
            //
        }

        private void RemoveLicenseLocally(ILicense license)
        {
            //
        }
    }
}
