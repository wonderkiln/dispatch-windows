using Dispatch.Helpers;
using Dispatch.Service.Updater;
using System;
using System.Diagnostics;
using System.Windows;

namespace Dispatch.ViewModels
{
    public class UpdateViewModel : Observable, IProgress<double>
    {
        public enum StatusType { None, Checking, UpdateAvailable, Downloading, Error }

        private StatusType status = StatusType.None;
        public StatusType Status
        {
            get
            {
                return status;
            }
            private set
            {
                status = value;
                Notify();
            }
        }

        private double progress = 0;
        public double Progress
        {
            get
            {
                return progress;
            }
            private set
            {
                progress = value;
                Notify();
            }
        }

        private string newVersion;
        public string NewVersion
        {
            get
            {
                return newVersion;
            }
            private set
            {
                newVersion = value;
                Notify();
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            private set
            {
                errorMessage = value;
                Notify();
            }
        }

        public RelayCommand DownloadAndInstallCommand { get; private set; }
        public RelayCommand ContactUsCommand { get; private set; }

        private readonly ApplicationUpdater applicationUpdater;

        public UpdateViewModel()
        {
            DownloadAndInstallCommand = new RelayCommand(DownloadAndInstall);
            ContactUsCommand = new RelayCommand(ContactUs);

            applicationUpdater = new ApplicationUpdater(new UpdateProvider(), this);
            CheckForUpdate();
        }

        private async void CheckForUpdate()
        {
            Status = StatusType.Checking;

            try
            {
                var update = await applicationUpdater.CheckForUpdate();
                Status = update != null ? StatusType.UpdateAvailable : StatusType.None;
                NewVersion = update?.Version.ToString();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Status = StatusType.Error;
            }
        }

        private async void DownloadAndInstall()
        {
            if (MessageBox.Show("Do you want to update to the latest version now?", "New update available", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Status = StatusType.Downloading;

                try
                {
                    await applicationUpdater.DownloadAndInstall();
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    Status = StatusType.Error;
                }
            }
        }

        public void Report(double value)
        {
            Progress = value;
        }

        private void ContactUs()
        {
            Process.Start("mailto:support@wonderkiln.com");
        }
    }
}
