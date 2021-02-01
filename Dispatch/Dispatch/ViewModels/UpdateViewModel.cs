using Dispatch.Helpers;
using Dispatch.Service.Updater;
using System;
using System.Windows;

namespace Dispatch.ViewModels
{
    public class UpdateViewModel : Observable, IProgress<double>
    {
        public enum StatusType { None, UpdateAvailable, Downloading }

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

        public RelayCommand DownloadAndInstallCommand { get; private set; }

        private readonly ApplicationUpdater applicationUpdater;

        public UpdateViewModel()
        {
            DownloadAndInstallCommand = new RelayCommand(DownloadAndInstall);
            applicationUpdater = new ApplicationUpdater(new UpdateProvider(), this);
            CheckForUpdate();
        }

        private async void CheckForUpdate()
        {
            var update = await applicationUpdater.CheckForUpdate();
            Status = update != null ? StatusType.UpdateAvailable : StatusType.None;
            NewVersion = update?.Version.ToString();
        }

        private void DownloadAndInstall()
        {
            if (MessageBox.Show("Do you want to update to the latest version now?", "New update available", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Status = StatusType.Downloading;
                applicationUpdater.DownloadAndInstall();
            }
        }

        public void Report(double value)
        {
            Progress = value;
        }
    }
}
