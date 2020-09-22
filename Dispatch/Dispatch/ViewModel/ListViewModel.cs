using Dispatch.Client;
using Dispatch.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dispatch.ViewModel
{
    public class ListViewModel : Observable
    {
        public IClient Client { get; private set; }

        public ListViewModel(IClient client)
        {
            Client = client;
            Load(client.RootPath);
        }

        private ObservableCollection<IResource> History { get; set; } = new ObservableCollection<IResource>();

        private bool _isBusy = false;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            private set
            {
                _isBusy = value;
                Notify();
            }
        }

        private IResource _currentResource;
        public IResource CurrentResource
        {
            get
            {
                return _currentResource;
            }
            private set
            {
                _currentResource = value;
                Notify();
            }
        }

        private List<IResource> _list;
        public List<IResource> List
        {
            get
            {
                return _list;
            }
            private set
            {
                _list = value;
                Notify();
            }
        }

        public async void Load(string path)
        {
            IsBusy = true;

            CurrentResource = await Client.Resource(path);
            List = await Client.Resources(path);
            History.Add(CurrentResource);

            IsBusy = false;
        }

        public async void Refresh()
        {
            IsBusy = true;

            if (CurrentResource != null)
            {
                List = await Client.Resources(CurrentResource.Path);
            }

            IsBusy = false;
        }

        public async void GoBack()
        {
            IsBusy = true;

            if (History.Count > 1)
            {
                History.RemoveAt(History.Count - 1);
                CurrentResource = History[History.Count - 1];
                List = await Client.Resources(CurrentResource.Path);
            }

            IsBusy = false;
        }

        public async Task Disconnect()
        {
            IsBusy = true;

            await Client.Disconnect();

            IsBusy = false;
        }

        public async void Delete(string path)
        {
            IsBusy = true;

            await Client.Delete(path);

            IsBusy = false;

            Refresh();
        }
    }
}
