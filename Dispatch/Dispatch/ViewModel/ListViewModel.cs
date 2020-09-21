using Dispatch.Client;
using Dispatch.Helpers;
using System;
using System.Collections.Generic;
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
            CurrentResource = await Client.Resource(path);
            List = await Client.Resources(path);
        }

        public async void Refresh()
        {
            if (CurrentResource != null)
            {
                List = await Client.Resources(CurrentResource.Path);
            }
        }

        public async Task Disconnect()
        {
            await Client.Disconnect();
        }
    }
}
