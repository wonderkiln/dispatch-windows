using Dispatch.Updater;
using Dispatch.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Screen
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<TabViewModel> Tabs { get; } = new ObservableCollection<TabViewModel>() { new TabViewModel() };

        public QueueViewModel QueueViewModel { get; } = new QueueViewModel();

        private ApplicationUpdater updater = new ApplicationUpdater(new AzureUpdateProvider());

        public MainWindow()
        {
            InitializeComponent();
            //_ = updater.CheckForUpdate();
        }

        private void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            var model = new TabViewModel();
            Tabs.Add(model);
            TabsListBox.SelectedItem = model;
        }

        private void CloseTabButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var model = button.DataContext as TabViewModel;
            Tabs.Remove(model);

            if (Tabs.Count == 0)
            {
                Tabs.Add(new TabViewModel());
            }
        }

        private void TabListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            if (e.AddedItems.Count == 0)
            {
                listBox.SelectedIndex = 0;
            }
            else
            {
                listBox.ScrollIntoView(e.AddedItems[0]);
            }
        }
    }
}
