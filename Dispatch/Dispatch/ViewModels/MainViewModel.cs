﻿using Dispatch.Fragments;
using Dispatch.Helpers;
using Dispatch.Service.Model;
using System;
using System.Collections.ObjectModel;

namespace Dispatch.ViewModels
{
    public class MainViewModel : Observable
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

        public RelayCommand<bool> NextTabCommand { get; }
        public RelayCommand<object> AddTabCommand { get; }
        public RelayCommand<TabViewModel> RemoveTabCommand { get; }
        public RelayCommand<object> OpenQueueCommand { get; }
        public RelayCommand<object> OpenMoreCommand { get; }

        public MainViewModel()
        {
            NextTabCommand = new RelayCommand<bool>(NextTab);
            AddTabCommand = new RelayCommand(AddTab);
            RemoveTabCommand = new RelayCommand<TabViewModel>(RemoveTab);
            OpenQueueCommand = new RelayCommand<object>(OpenQueue);
            OpenMoreCommand = new RelayCommand<object>(OpenMore);

            NewTab();
        }

        private void NextTab(bool forward)
        {
            if (SelectedTab != null)
            {
                var index = OpenTabs.IndexOf(SelectedTab);

                if (forward)
                {
                    var newIndex = index + 1 < OpenTabs.Count ? index + 1 : 0;
                    SelectedTab = OpenTabs[newIndex];
                }
                else
                {
                    var newIndex = index - 1 >= 0 ? index - 1 : OpenTabs.Count - 1;
                    SelectedTab = OpenTabs[newIndex];
                }
            }
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
                Bookmarks.Add(new Bookmark(resource));
            }
        }

        private void AddTab()
        {
            NewTab();
        }

        private void RemoveTab(TabViewModel item)
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
