using Dispatch.Helpers;
using Dispatch.Service.Updater;

namespace Dispatch.ViewModel
{
    public class UpdateViewModel : Observable
    {
        public bool HasUpdate
        {
            get
            {
                return applicationUpdater.HasUpdate;
            }
        }

        public RelayCommand UpdateCommand { get; private set; }

        private ApplicationUpdater applicationUpdater = new ApplicationUpdater(new UpdateProvider());

        public UpdateViewModel()
        {
            UpdateCommand = new RelayCommand(Update);
            Check();
        }

        private async void Check()
        {
            await applicationUpdater.CheckForUpdate(true);
            Notify("HasUpdate ");
        }

        private void Update(object arg)
        {
            applicationUpdater.DownloadAndInstall();
        }
    }
}
