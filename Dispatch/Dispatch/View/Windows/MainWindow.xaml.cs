using Dispatch.Helpers;
using Dispatch.Updater;
using Dispatch.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Windows
{
    public partial class MainWindow : Window
    {
        public TabsViewModel ViewModel { get; } = new TabsViewModel();

        private void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            TabsListBox.SelectedItem = ViewModel.NewTab();
        }

        private void CloseTabButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var model = button.DataContext as TabViewModel;
            model.Disconnect();
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

        private async void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var updater = new ApplicationUpdater(new AzureUpdateProvider());
            await updater.CheckForUpdate();
        }

        private void TransfersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TransfersPopup.IsOpen = true;
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"You have version {Constants.VERSION} ({Constants.CHANNEL})", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
