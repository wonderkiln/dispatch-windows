using Dispatch.Updater;
using Dispatch.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Screen
{
    public partial class MainWindow : Window
    {

        private ApplicationUpdater updater = new ApplicationUpdater(new AzureUpdateProvider());

        public TabsViewModel ViewModel { get; } = new TabsViewModel();

        private async void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            TabsListBox.SelectedItem = ViewModel.NewTab();
        }

        private void CloseTabButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var model = button.DataContext as TabViewModel;
            ViewModel.CloseTab(model);
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

        private void UpdatesButton_Click(object sender, RoutedEventArgs e)
        {
            _ = updater.CheckForUpdate();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            p.IsOpen = true;
        }
    }
}
