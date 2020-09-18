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
        private List<string> history = new List<string>();

        public IClient Client { get; private set; }

        public RelayCommand DisconnectCommand { get; private set; }


        private string _currentPath;
        public string CurrentPath
        {
            get
            {
                return _currentPath;
            }
            private set
            {
                _currentPath = value;
                Notify();
            }
        }

        private List<Resource> _list;
        public List<Resource> List
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

        public ListViewModel(IClient client)
        {
            Client = client;
            Load(client.Root);
        }

        public async void Load(string path)
        {
            List = await Client.List(path);

            if (CurrentPath != null) history.Add(CurrentPath);
            CurrentPath = path;
        }

        public async Task Disconnect()
        {
            await Client.Disconnect();
        }

        public async void Up()
        {
            if (history.Any())
            {
                var path = history[history.Count - 1];
                history.RemoveAt(history.Count - 1);

                List = await Client.List(path);

                CurrentPath = path;
            }
        }
    }
}
