using Dispatch.Helpers;
using Dispatch.Service.API;
using Dispatch.Service.Licensing;
using Dispatch.Service.Models;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Dispatch.ViewModels
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

        public RelayCommand InstallTrialCommand { get; }
        public RelayCommand<string> InstallLicenseCommand { get; }
        public RelayCommand RemoveLicenseCommand { get; }

        private string GenerateFingerprint()
        {
            string fingerprint = null;

            var key = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{Constants.APP_NAME}");

            try
            {
                fingerprint = key.GetValue("Fingerprint") as string;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (string.IsNullOrEmpty(fingerprint))
            {
                fingerprint = Fingerprint.GetFingerprint();
                key.SetValue("Fingerprint", fingerprint, RegistryValueKind.String);
            }

            key.Close();

            return fingerprint;
        }

        public LicenseViewModel()
        {
            client = new APIClient(GenerateFingerprint());

            InstallTrialCommand = new RelayCommand(InstallTrial);
            InstallLicenseCommand = new RelayCommand<string>(InstallLicense);
            RemoveLicenseCommand = new RelayCommand(RemoveLicense);

            Console.WriteLine("License manager initialized with hardware identifier: {0}", client.HardwareIdentifier);

            _ = Load();
        }

        public async Task Load()
        {
            Device = await client.GetDeviceStatus();

            InstallTrialCommand.IsExecutable = Device.Status == DeviceStatus.LicenseStatus.None;
            InstallLicenseCommand.IsExecutable = Device.Status <= DeviceStatus.LicenseStatus.TrialExpired;
            RemoveLicenseCommand.IsExecutable = Device.Status >= DeviceStatus.LicenseStatus.License;

            // TODO: If the status is none, trial expired, license expired wrong version show the blocking modal
            // TODO: Do not update if license expired and major version differs
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

        private async void InstallLicense(string key)
        {
            try
            {
                await client.InstallLicense(key);
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
                await client.RemoveLicense(Device.Device?.License?.Key);
                await Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
