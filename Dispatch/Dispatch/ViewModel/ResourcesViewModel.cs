using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Dispatch.ViewModel
{
    public class ResourcesViewModel : Observable
    {
        public IClient Client { get; private set; }

        private RelayCommand<object> _backCommand;
        public RelayCommand<object> BackCommand
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

        private RelayCommand<object> _homeCommand;
        public RelayCommand<object> HomeCommand
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

        private RelayCommand<object> _refreshCommand;
        public RelayCommand<object> RefreshCommand
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

        private string InitialPath;

        private Resource[] selectedResources;
        public Resource[] SelectedResources
        {
            get
            {
                return selectedResources;
            }
            set
            {
                selectedResources = value;
                Notify();
            }
        }

        public RelayCommand<IEnumerable<object>> AddBookmarkCommand { get; }
        public RelayCommand<Resource> NavigateCommand { get; }

        public ResourcesViewModel(IClient client, string initialPath)
        {
            AddBookmarkCommand = new RelayCommand<IEnumerable<object>>(AddBookmark);
            NavigateCommand = new RelayCommand<Resource>(Navigate);

            Client = client;
            BackCommand = new RelayCommand<object>(BackCommandAction, false);
            HomeCommand = new RelayCommand<object>(HomeCommandAction);
            RefreshCommand = new RelayCommand<object>(RefreshCommandAction);

            InitialPath = initialPath;

            Load(InitialPath);
        }

        public event EventHandler<Resource[]> OnAddBookmark;

        private void AddBookmark(IEnumerable<object> items)
        {
            var bookmarks = items.Cast<Resource>().ToArray();
            OnAddBookmark?.Invoke(this, bookmarks);
        }

        private void Navigate(Resource item)
        {
            if (item.Type != ResourceType.File)
            {
                Load(item.Path);
            }
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
                if (value != null)
                {
                    _resources = value.OrderBy(e => e.Type).ThenBy(e => e.Name).ToArray();
                }
                else
                {
                    _resources = value;
                }

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
