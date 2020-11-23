using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Dispatch.ViewModel
{
    public class ListViewModel : Observable
    {
        public IClient Client { get; private set; }

        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get
            {
                return _backCommand;
            }
            private set
            {
                _backCommand = value;
                Notify();
            }
        }

        public ListViewModel(IClient client)
        {
            Client = client;
            BackCommand = new RelayCommand(BackCommandAction, false);
            
            Load(client.InitialPath);
        }

        private void BackCommandAction(object parameter)
        {
            Back();
        }

        private readonly Stack<Resource> History = new Stack<Resource>();

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

        private Resource[] _resources;
        public Resource[] Resources
        {
            get
            {
                return _resources;
            }
            private set
            {
                _resources = value;
                Notify();
            }
        }

        public async void Load(string path)
        {
            try
            {
                var current = await Client.FetchResource(path);
                var resources = await Client.FetchResources(path);

                if (Current != null)
                {
                    History.Push(Current);
                }

                Current = current;
                Resources = resources;
                BackCommand.IsExecutable = History.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void Back()
        {
            if (History.Count == 0)
            {
                return;
            }

            try
            {
                var current = History.Pop();
                var resources = await Client.FetchResources(current.Path);

                Current = current;
                Resources = resources;
                BackCommand.IsExecutable = History.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
