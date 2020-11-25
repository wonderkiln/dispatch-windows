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

        private RelayCommand _homeCommand;
        public RelayCommand HomeCommand
        {
            get
            {
                return _homeCommand;
            }
            private set
            {
                _homeCommand = value;
                Notify();
            }
        }

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand;
            }
            private set
            {
                _refreshCommand = value;
                Notify();
            }
        }

        private string _temporaryPath;
        public string Path
        {
            get
            {
                return _temporaryPath ?? Current?.Path ?? "";
            }
            set
            {
                _temporaryPath = value;
                Load(value);
            }
        }

        public ListViewModel(IClient client)
        {
            Client = client;
            BackCommand = new RelayCommand(BackCommandAction, false);
            HomeCommand = new RelayCommand(HomeCommandAction);
            RefreshCommand = new RelayCommand(RefreshCommandAction);

            Load(client.InitialPath);
        }

        private void BackCommandAction(object parameter)
        {
            Back();
        }

        private void HomeCommandAction(object parameter)
        {
            Load(Client.InitialPath);
        }

        private void RefreshCommandAction(object parameter)
        {
            if (Current != null)
            {
                Load(Current.Path);
            }
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
                Notify("Path");

                RefreshCommand.IsExecutable = _current != null;
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

                if (Current != null && current.Path != Current.Path)
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
            finally
            {
                _temporaryPath = null;
                Notify("Path");
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
                Current = History.Pop();
                Resources = await Client.FetchResources(Current.Path);
                BackCommand.IsExecutable = History.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
