using Dispatch.Service.Storage;
using Dispatch.Service.Models;
using System.Linq;

namespace Dispatch.Service.Licensing
{
    public class LicenseWriter
    {
        private readonly Storage<License[]> storage = new Storage<License[]>("License.json");

        public License[] GetAllLicenses()
        {
            return storage.Load(new License[] { });
        }

        public void AddLicense(License license)
        {
            var licenses = GetAllLicenses().ToList();
            licenses.Add(license);
            storage.Save(licenses.ToArray());
        }

        public void RemoveLicense(License license)
        {
            var licenses = GetAllLicenses().ToList();
            licenses.RemoveAll(e => e.Id == license.Id);
            storage.Save(licenses.ToArray());
        }

        public void ReplaceLicense(License license, License withLicense)
        {
            var licenses = GetAllLicenses().ToList();
            licenses.RemoveAll(e => e.Id == license.Id);
            licenses.Add(withLicense);
            storage.Save(licenses.ToArray());
        }
    }
}
