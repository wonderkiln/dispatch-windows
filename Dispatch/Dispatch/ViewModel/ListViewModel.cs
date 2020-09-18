﻿using Dispatch.Client;
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

        public RelayCommand DisconnectCommand { get; private set; }

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
        }

        public async Task Disconnect()
        {
            await Client.Disconnect();
        }
    }
}
