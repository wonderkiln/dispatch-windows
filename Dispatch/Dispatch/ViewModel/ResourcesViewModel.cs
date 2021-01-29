using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Dispatch.ViewModel
{
    public class ResourcesDragDropHandler : IDragSource, IDropTarget
    {
        private readonly ResourcesViewModel viewModel;

        public ResourcesDragDropHandler(ResourcesViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        public void DragCancelled()
        {
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
        }

        public void Dropped(IDropInfo dropInfo)
        {
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            var sourceItems = (IEnumerable<object>)dragInfo.SourceItems;

            foreach (var item in sourceItems)
            {
                if (!(item is Resource))
                {
                    dragInfo.Effects = DragDropEffects.None;
                    return;
                }
            }

            dragInfo.Data = sourceItems.Cast<Resource>();
            dragInfo.Effects = DragDropEffects.Link | DragDropEffects.Copy;
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IEnumerable<Resource> resources)
            {
                foreach (var resource in resources)
                {
                    if (resource.Client == viewModel.Client)
                    {
                        return;
                    }
                }

                dropInfo.DropTargetAdorner = null;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IEnumerable<Resource> resources)
            {
                viewModel.Transfer(resources.ToArray());
            }
        }
    }

    public class ResourcesViewModel : Observable
    {
        private readonly Stack<string> history = new Stack<string>();
        private readonly string initialPath;

        public ResourcesDragDropHandler DragDropHandler { get; }

        public IClient Client { get; }

        public RelayCommand<object> BackCommand { get; }
        public RelayCommand<object> HomeCommand { get; }
        public RelayCommand<object> RefreshCommand { get; }
        public RelayCommand<IEnumerable<object>> AddBookmarkCommand { get; }
        public RelayCommand<IEnumerable<object>> DeleteCommand { get; }
        public RelayCommand<Resource> NavigateCommand { get; }

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
                    // Folders first
                    _resources = value.OrderBy(e => e.Type).ThenBy(e => e.Name).ToArray();
                }
                else
                {
                    _resources = value;
                }

                Notify();
            }
        }

        public event EventHandler<Resource[]> OnAddBookmark;

        public ResourcesViewModel(IClient client, string initialPath)
        {
            DragDropHandler = new ResourcesDragDropHandler(this);

            BackCommand = new RelayCommand<object>(Back, false);
            HomeCommand = new RelayCommand<object>(Home);
            RefreshCommand = new RelayCommand<object>(Refresh);
            AddBookmarkCommand = new RelayCommand<IEnumerable<object>>(AddBookmark);
            DeleteCommand = new RelayCommand<IEnumerable<object>>(Delete);
            NavigateCommand = new RelayCommand<Resource>(Navigate);

            Client = client;
            this.initialPath = initialPath;

            Load(this.initialPath);
        }

        private void Back(object parameter)
        {
            Back();
        }

        private void Home(object parameter)
        {
            Load(initialPath);
        }

        private void Refresh(object parameter)
        {
            Refresh();
        }

        private void AddBookmark(IEnumerable<object> items)
        {
            var resources = items.Cast<Resource>().ToArray();
            OnAddBookmark?.Invoke(this, resources);
        }

        private void Delete(IEnumerable<object> items)
        {
            var resources = items.Cast<Resource>().ToArray();
            var names = resources.Select(e => $"- {e.Name}\n");

            if (MessageBox.Show($"Are you sure you want to delete the following items?\n\n{string.Join("", names)}", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var resource in resources)
                {
                    ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Delete, resource, null, this));
                }
            }
        }

        private void Navigate(Resource item)
        {
            if (item.Type != ResourceType.File)
            {
                Load(item.Path);
            }
        }

        public void Refresh()
        {
            if (CurrentPath != null)
            {
                Load(CurrentPath);
            }
        }

        public async void Load(string path)
        {
            try
            {
                var resources = await Client.FetchResources(path);

                if (CurrentPath != null && path != CurrentPath)
                {
                    history.Push(CurrentPath);
                }

                CurrentPath = path;
                Resources = resources;
                BackCommand.IsExecutable = history.Count > 0;
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

        private async void Back()
        {
            if (history.Count == 0)
            {
                return;
            }

            try
            {
                CurrentPath = history.Pop();
                Resources = await Client.FetchResources(CurrentPath);
                BackCommand.IsExecutable = history.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void Transfer(Resource[] resources)
        {
            var currentResource = await Client.FetchResource(_currentPath);

            foreach (var resource in resources)
            {
                if (Client is LocalClient)
                {
                    ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Download, resource, currentResource, this));
                }
                else
                {
                    ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Upload, resource, currentResource, this));
                }
            }
        }

        public async Task Disconnect()
        {
            await Client.Diconnect();
        }
    }
}
