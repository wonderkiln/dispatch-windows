using Dispatch.Helpers;
using Dispatch.Service.Model;
using Dispatch.View.Fragments;
using System;
using System.Collections.ObjectModel;

namespace Dispatch.ViewModel
{
    public class RootViewModel : Observable
    {
        public BookmarksViewModel Bookmarks { get; } = new BookmarksViewModel();

        public QueueViewModel Queue { get; } = new QueueViewModel();

        public UpdateViewModel Update { get; } = new UpdateViewModel();

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

        private string sidebarTitle;
        public string SidebarTitle
        {
            get
            {
                return sidebarTitle;
            }
            set
            {
                sidebarTitle = value;
                Notify();
            }
        }

        public bool IsSidebarOpen
        {
            get
            {
                return sidebarContent != null;
            }
            set
            {
                if (!value)
                {
                    sidebarContent = null;
                }

                Notify();
            }
        }

        public RelayCommand<object> AddTabCommand { get; }
        public RelayCommand<TabViewModel> CloseTabCommand { get; }
        public RelayCommand<object> OpenQueueCommand { get; }
        public RelayCommand<object> OpenMoreCommand { get; }

        public RootViewModel()
        {
            AddTabCommand = new RelayCommand<object>(AddTab);
            CloseTabCommand = new RelayCommand<TabViewModel>(CloseTab);
            OpenQueueCommand = new RelayCommand<object>(OpenQueue);
            OpenMoreCommand = new RelayCommand<object>(OpenMore);

            NewTab();
        }

        private void NewTab()
        {
            var tab = new TabViewModel();
            tab.OnAddBookmark += Tab_OnAddBookmark;
            OpenTabs.Add(tab);
            SelectedTab = tab;
        }

        private void Tab_OnAddBookmark(object sender, Resource[] e)
        {
            foreach (var resource in e)
            {
                Bookmarks.Add(new BookmarkItem(resource));
            }
        }

        private void AddTab(object parameter)
        {
            NewTab();
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

        private void OpenQueue(object parameter)
        {
            SidebarTitle = "Transfers";
            SidebarContent = new QueueView() { DataContext = Queue };
        }

        private void OpenMore(object parameter)
        {
            SidebarTitle = "More";
            var moreView = new MoreView();
            moreView.OnChangeSidebar += MoreView_OnChangeSidebar;
            SidebarContent = moreView;
        }

        private void MoreView_OnChangeSidebar(object sender, object e)
        {
            SidebarContent = e;
        }
    }
}
