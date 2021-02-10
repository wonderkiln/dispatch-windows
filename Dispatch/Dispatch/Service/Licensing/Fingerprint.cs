using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Dispatch.Service.Licensing
{
    public class Fingerprint
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
}
