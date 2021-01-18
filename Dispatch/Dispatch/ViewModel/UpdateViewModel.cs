using Dispatch.Helpers;
using Dispatch.Service.Updater;
using System.Windows;

namespace Dispatch.ViewModel
{
    public class UpdateViewModel : Observable
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

        public RelayCommand DownloadAndInstallCommand { get; private set; }

        private readonly ApplicationUpdater applicationUpdater = new ApplicationUpdater(new UpdateProvider());

        public UpdateViewModel()
        {
            applicationUpdater.DownloadProgressChanged += ApplicationUpdater_DownloadProgressChanged;
            DownloadAndInstallCommand = new RelayCommand(DownloadAndInstall);
            CheckForUpdates();
        }

        private void ApplicationUpdater_DownloadProgressChanged(object sender, double e)
        {
            Progress = e;
        }

        private async void CheckForUpdates()
        {
            await applicationUpdater.CheckForUpdate(true);

            if (applicationUpdater.HasUpdate)
            {
                Status = StatusType.UpdateAvailable;
            }
        }

        private void DownloadAndInstall(object arg)
        {
            if (MessageBox.Show($"Do you want to update to the latest version now?\nYou have {Constants.VERSION} and the latest version is {applicationUpdater.LatestUpdate.Version}.", "New update available", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Status = StatusType.Downloading;
                applicationUpdater.DownloadAndInstall();
            }
        }
    }
}
