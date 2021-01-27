using Dispatch.Helpers;
using Dispatch.Service.Model;
using System.Collections.ObjectModel;

namespace Dispatch.ViewModel
{
    public class RootViewModel : Observable
    {
        public StorageViewModel<BookmarkItem> Bookmarks { get; } = new StorageViewModel<BookmarkItem>("Bookmarks.json");

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

        public RelayCommand<TabViewModel> CloseTabCommand { get; }

        public RootViewModel()
        {
            CloseTabCommand = new RelayCommand<TabViewModel>(CloseTab);

            NewTab();
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
            // TODO: Disconnect

            OpenTabs.Remove(item);

            if (OpenTabs.Count == 0)
            {
                NewTab();
            }
        }
    }
}
