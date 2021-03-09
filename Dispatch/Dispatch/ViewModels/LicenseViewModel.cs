using Dispatch.Helpers;
using Dispatch.Service.API;
using Dispatch.Service.Models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dispatch.Service.Licensing
{
    public class LicenseViewModel : Observable
    {
        private readonly APIClient client;

        private DeviceStatus device;
        public DeviceStatus Device
        {
            get
            {
                return device;
            }
            set
            {
                device = value;
                Notify();
            }
        }

        private string licenseKey;
        public string LicenseKey
        {
            get
            {
                return licenseKey;
            }
            set
            {
                licenseKey = value;
                Notify();
            }
        }

        public RelayCommand InstallTrialCommand { get; }
        public RelayCommand InstallLicenseCommand { get; }
        public RelayCommand RemoveLicenseCommand { get; }

        private string GenerateFingerprint()
        {
            string fingerprint = null;

            try
            {
                fingerprint = File.ReadAllText("fingerprint.txt", Encoding.UTF8);
            }
            catch
            {
                //
            }

            if (string.IsNullOrEmpty(fingerprint))
            {
                fingerprint = Fingerprint.GetFingerprint();
                File.WriteAllText("fingerprint.txt", fingerprint, Encoding.UTF8);
            }

            return fingerprint;
        }

        public LicenseViewModel()
        {
            client = new APIClient(GenerateFingerprint());

            InstallTrialCommand = new RelayCommand(InstallTrial);
            InstallLicenseCommand = new RelayCommand(InstallLicense);
            RemoveLicenseCommand = new RelayCommand(RemoveLicense);

            Console.WriteLine("License manager initialized with hardware identifier: {0}", client.HardwareIdentifier);
        }

        public async Task Load()
        {
            Device = await client.GetDeviceStatus();
            LicenseKey = Device.Device?.License?.Key;

            InstallTrialCommand.IsExecutable = Device.Status == DeviceStatus.LicenseStatus.None;
            InstallLicenseCommand.IsExecutable = Device.Status <= DeviceStatus.LicenseStatus.TrialExpired;
            RemoveLicenseCommand.IsExecutable = Device.Status >= DeviceStatus.LicenseStatus.License;
        }

        private async void InstallTrial()
        {
            try
            {
                await client.InstallTrial();
                await Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void InstallLicense()
        {
            try
            {
                await client.InstallLicense(LicenseKey);
                await Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RemoveLicense()
        {
            try
            {
                await client.RemoveLicense(LicenseKey);
                await Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
