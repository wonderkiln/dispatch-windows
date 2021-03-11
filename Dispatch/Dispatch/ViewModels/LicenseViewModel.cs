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
                Notify("Status");
            }
        }

        public DeviceStatus.LicenseStatus Status
        {
            get
            {
                return Device?.Status ?? DeviceStatus.LicenseStatus.None;
            }
        }

        public RelayCommand InstallTrialCommand { get; }
        public RelayCommand<string> InstallLicenseCommand { get; }
        public RelayCommand RemoveLicenseCommand { get; }
        public RelayCommand EnterLicenseCommand { get; }

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
            EnterLicenseCommand = new RelayCommand(EnterLicense);

            Console.WriteLine("License manager initialized with hardware identifier: {0}", client.HardwareIdentifier);

            _ = Load();
        }

        public async Task Load()
        {
            try
            {
                // TODO: Load from cache?
                Device = await client.GetDeviceStatus();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            InstallTrialCommand.IsExecutable = Status == DeviceStatus.LicenseStatus.None;
            InstallLicenseCommand.IsExecutable = Status <= DeviceStatus.LicenseStatus.TrialExpired;
            RemoveLicenseCommand.IsExecutable = Status >= DeviceStatus.LicenseStatus.License;

            // If the status is none, trial expired or license expired wrong version then show the blocking license modal
            if (Status == DeviceStatus.LicenseStatus.None ||
                Status == DeviceStatus.LicenseStatus.TrialExpired ||
                Status == DeviceStatus.LicenseStatus.LicenseExpiredWrongVersion)
            {
                EnterLicense();
            }
            else
            {
                licenseWindow?.Close();
                licenseWindow = null;
            }

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

        private Window licenseWindow;

        private void EnterLicense()
        {
            if (licenseWindow != null)
            {
                return;
            }

            var mainWindow = Application.Current.MainWindow;

            if (!mainWindow.IsLoaded)
            {
                mainWindow.Loaded += MainWindow_Loaded;
                return;
            }

            var window = new LicenseWindow();
            window.Owner = mainWindow;
            window.DataContext = this;

            licenseWindow = window;

            window.ShowDialog();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = (Window)sender;
            mainWindow.Loaded -= MainWindow_Loaded;
            EnterLicense();
        }
    }
}
