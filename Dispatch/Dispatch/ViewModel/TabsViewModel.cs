using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dispatch.ViewModel
{
    public class TabsViewModel
    {
        public ObservableCollection<TabViewModel> Tabs { get; } = new ObservableCollection<TabViewModel>();

        public TabsViewModel()
        {
            NewTab();
        }

        public TabViewModel NewTab()
        {
            var tab = new TabViewModel
            {
                Icon = new BitmapImage(new Uri("/Resources/ic_bolt.png", UriKind.Relative)),
                Title = "New Connection"
            };

            Tabs.Add(tab);

            return tab;
        }

        public void CloseTab(TabViewModel tab)
        {
            // tab.Disconnect();

            Tabs.Remove(tab);

            if (Tabs.Count == 0)
            {
                NewTab();
            }
        }
    }
}
