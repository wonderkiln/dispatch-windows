using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using System;
using System.Windows;

namespace Dispatch.ViewModel
{
    public class ListViewModel : Observable
    {
        public IClient Client { get; private set; }

        public ListViewModel(IClient client)
        {
            Client = client;
            Load(client.InitialPath);
        }

        private Resource _current;
        public Resource Current
        {
            get
            {
                return _current;
            }
            private set
            {
                _current = value;
                Notify();
            }
        }

        private Resource[] _list;
        public Resource[] Resources
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
            try
            {
                Current = await Client.FetchResource(path);
                Resources = await Client.FetchResources(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
