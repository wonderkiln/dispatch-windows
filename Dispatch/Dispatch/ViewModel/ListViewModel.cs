using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using Dispatch.Service.Storage;
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
                return _temporaryPath ?? CurrentPath ?? "";
            }
            set
            {
                _temporaryPath = value;
                Load(value);
            }
        }

        public FavoritesStorage Favorites { get; private set; }

        private string InitialPath;

        public string Title { get; private set; }

        public ListViewModel(IClient client, string initialPath, string title)
        {
            Client = client;
            BackCommand = new RelayCommand(BackCommandAction, false);
            HomeCommand = new RelayCommand(HomeCommandAction);
            RefreshCommand = new RelayCommand(RefreshCommandAction);

            InitialPath = initialPath;

            Favorites = new FavoritesStorage(title);

            Title = title;

            Load(InitialPath);
        }

        private void BackCommandAction(object parameter)
        {
            Back();
        }

        private void HomeCommandAction(object parameter)
        {
            Load(InitialPath);
        }

        private void RefreshCommandAction(object parameter)
        {
            Refresh();
        }

        public void Refresh()
        {
            if (CurrentPath != null)
            {
                Load(CurrentPath);
            }
        }

        private readonly Stack<string> History = new Stack<string>();

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
                Notify("Path");

                RefreshCommand.IsExecutable = _currentPath != null;
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
                var resources = await Client.FetchResources(path);

                if (CurrentPath != null && path != CurrentPath)
                {
                    History.Push(CurrentPath);
                }

                CurrentPath = path;
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
                CurrentPath = History.Pop();
                Resources = await Client.FetchResources(CurrentPath);
                BackCommand.IsExecutable = History.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
