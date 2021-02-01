using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Dispatch.ViewModels
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
        public RelayCommand<IEnumerable<object>> TransferCommand { get; }

        private string path;
        public string Path
        {
            get
            {
                return path;
            }
            private set
            {
                path = value;
                Notify();
                Notify("DisplayPath");

                Load(value);
            }
        }

        private class Pair<A, B>
        {
            public A Left { get; }

            public B Right { get; }

            public Pair(A left, B right)
            {
                Left = left;
                Right = right;
            }
        }

        private static readonly List<Pair<string, string>> DisplayPathMap = new List<Pair<string, string>>()
        {
            new Pair<string, string>(LocalClient.AllDrivesPathKey, "\\"),
        };

        public string DisplayPath
        {
            get
            {
                var pair = DisplayPathMap.Find(e => e.Left == path);
                if (pair != null)
                {
                    return pair.Right;
                }

                return path;
            }
            set
            {
                PushHistory(Path);

                var pair = DisplayPathMap.Find(e => e.Right == value);
                if (pair != null)
                {
                    Path = pair.Left;
                }
                else
                {
                    Path = value;
                }
            }
        }

        private Resource[] resources;
        public Resource[] Resources
        {
            get
            {
                return resources;
            }
            private set
            {
                if (value != null)
                {
                    // Folders first
                    resources = value.OrderBy(e => e.Type).ThenBy(e => e.Name).ToArray();
                }
                else
                {
                    resources = value;
                }

                Notify();
            }
        }

        public event EventHandler<Resource[]> OnAddBookmark;

        private ResourcesViewModel side;
        public ResourcesViewModel Side
        {
            get
            {
                return side;
            }
            set
            {
                side = value;
                Notify();
            }
        }

        public ResourcesViewModel(IClient client, string initialPath)
        {
            DragDropHandler = new ResourcesDragDropHandler(this);

            BackCommand = new RelayCommand<object>(Back, false);
            HomeCommand = new RelayCommand<object>(Home);
            RefreshCommand = new RelayCommand<object>(Refresh);
            AddBookmarkCommand = new RelayCommand<IEnumerable<object>>(AddBookmark);
            DeleteCommand = new RelayCommand<IEnumerable<object>>(Delete);
            NavigateCommand = new RelayCommand<Resource>(Navigate);
            TransferCommand = new RelayCommand<IEnumerable<object>>(Transfer);

            this.initialPath = !string.IsNullOrEmpty(initialPath) ? initialPath : "/";

            Client = client;
            Path = initialPath;
        }

        private void Back(object parameter)
        {
            if (history.Count == 0)
            {
                return;
            }

            Path = PopHistory();
        }

        private void Home(object parameter)
        {
            PushHistory(Path);
            Path = initialPath;
        }

        private void Refresh(object parameter)
        {
            Refresh();
        }

        public void Refresh()
        {
            Load(Path);
        }

        private void AddBookmark(IEnumerable<object> items)
        {
            var resources = items.Cast<Resource>().ToArray();
            OnAddBookmark?.Invoke(this, resources);
        }

        private void Delete(IEnumerable<object> items)
        {
            var resources = items.Cast<Resource>().ToArray();
            if (resources.Length == 0) return;

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
            if (item != null && item.Type != ResourceType.File)
            {
                PushHistory(Path);
                Path = item.Path;
            }
        }

        private void PushHistory(string path)
        {
            history.Push(path);
            BackCommand.IsExecutable = history.Count > 0;
        }

        private string PopHistory()
        {
            var path = history.Pop();
            BackCommand.IsExecutable = history.Count > 0;
            return path;
        }

        private async void Load(string path)
        {
            try
            {
                Resources = null;
                Resources = await Client.FetchResources(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Transfer(IEnumerable<object> items)
        {
            var resources = items.Cast<Resource>().ToArray();
            Transfer(resources);
        }

        public async void Transfer(Resource[] resources)
        {
            var currentResource = await Client.FetchResource(Path);

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
