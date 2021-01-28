using Dispatch.Helpers;
using Dispatch.Service.Model;
using Dispatch.View.Fragments;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Dispatch.ViewModel
{
    public class BookmarksDropTarget : IDragSource, IDropTarget
    {
        private BookmarksViewModel viewModel;

        public BookmarksDropTarget(BookmarksViewModel viewModel)
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
                if (!(item is BookmarkItem))
                {
                    dragInfo.Effects = DragDropEffects.None;
                    return;
                }
            }

            dragInfo.Data = sourceItems.Cast<BookmarkItem>();
            dragInfo.Effects = DragDropEffects.Move;
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IEnumerable<BookmarkItem>)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
            else if (dropInfo.Data is IEnumerable<Resource> resources)
            {
                foreach (var resource in resources)
                {
                    if (resource.Type == ResourceType.File)
                    {
                        return;
                    }
                }

                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Link;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IEnumerable<BookmarkItem> bookmarks)
            {
                foreach (var bookmark in bookmarks)
                {
                    viewModel.Move(dropInfo.InsertIndex, bookmark);
                }
            }
            else if (dropInfo.Data is IEnumerable<Resource> resources)
            {
                foreach (var resource in resources)
                {
                    viewModel.Insert(dropInfo.InsertIndex, new BookmarkItem(resource));
                }
            }
        }
    }

    public class BookmarksViewModel : StorageViewModel<BookmarkItem>
    {
        public BookmarksDropTarget DragDropHandler { get; }

        public BookmarksViewModel() : base("Bookmarks.json")
        {
            DragDropHandler = new BookmarksDropTarget(this);
        }
    }

    public class RootViewModel : Observable
    {
        public BookmarksViewModel Bookmarks { get; } = new BookmarksViewModel();

        public ObservableCollection<TabViewModel> OpenTabs { get; } = new ObservableCollection<TabViewModel>();

        private TabViewModel selectedTab;
        public TabViewModel SelectedTab
        {
            get
            {
                return selectedTab;
            }
            set
            {
                selectedTab = value;
                Notify();
            }
        }

        private object sidebarContent;
        public object SidebarContent
        {
            get
            {
                return sidebarContent;
            }
            set
            {
                sidebarContent = value;
                Notify();
                Notify("IsSidebarOpen");
            }
        }

        public bool IsSidebarOpen
        {
            get
            {
                return sidebarContent != null;
            }
        }

        public RelayCommand<object> AddTabCommand { get; }
        public RelayCommand<TabViewModel> CloseTabCommand { get; }
        public RelayCommand<object> OpenMoreCommand { get; }

        public RootViewModel()
        {
            AddTabCommand = new RelayCommand<object>(AddTab);
            CloseTabCommand = new RelayCommand<TabViewModel>(CloseTab);
            OpenMoreCommand = new RelayCommand<object>(OpenMore);

            NewTab();
        }

        private void AddTab(object parameter)
        {
            NewTab();
        }

        private void OpenMore(object parameter)
        {
            SidebarContent = new SettingsView();
        }

        public TabViewModel NewTab()
        {
            var tab = new TabViewModel();
            tab.OnAddBookmark += Tab_OnAddBookmark;
            OpenTabs.Add(tab);
            SelectedTab = tab;

            return tab;
        }

        private void Tab_OnAddBookmark(object sender, Resource[] e)
        {
            foreach (var resource in e)
            {
                Bookmarks.Add(new BookmarkItem(resource));
            }
        }

        private void CloseTab(TabViewModel item)
        {
            item.Disconnect();

            var index = OpenTabs.IndexOf(item);
            var prevIndex = Math.Max(0, index - 1);

            OpenTabs.RemoveAt(index);

            if (OpenTabs.Count == 0)
            {
                NewTab();
            }

            if (SelectedTab == null)
            {
                SelectedTab = OpenTabs[prevIndex];
            }
        }
    }
}
