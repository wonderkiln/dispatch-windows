using System.Collections.ObjectModel;

namespace Dispatch.ViewModel
{
    public class TabsViewModel
    {
        public ObservableCollection<TabViewModel> Tabs { get; }
            = new ObservableCollection<TabViewModel>() { new TabViewModel() };

        public TabViewModel NewTab()
        {
            var tab = new TabViewModel();
            Tabs.Add(tab);

            return tab;
        }

        public void CloseTab(TabViewModel tab)
        {
            // TODO: tab.Disconnect();

            Tabs.Remove(tab);

            if (Tabs.Count == 0)
            {
                NewTab();
            }
        }
    }
}
