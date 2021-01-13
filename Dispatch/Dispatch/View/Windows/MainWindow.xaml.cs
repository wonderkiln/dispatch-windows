using Dispatch.Controls;
using Dispatch.Helpers;
using Dispatch.View.Fragments;
using Dispatch.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Windows
{
    public partial class MainWindow : Window
    {
        public TabsViewModel ViewModel { get; } = new TabsViewModel();

        public QueueViewModel QueueViewModel { get; } = new QueueViewModel();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.EnableBlurForWindow(this);
        }

        private void TabListBox_OnAdd(object sender, EventArgs e)
        {
            TabListBox.SelectedItem = ViewModel.NewTab();
        }

        private void TabListBox_OnClose(object sender, EventArgs e)
        {
            var item = (DPTabListBoxItem)sender;
            ViewModel.CloseTab((TabViewModel)item.DataContext);
        }

        private void TabListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                ((DPTabListBox)sender).SelectedIndex = 0;
            }
        }

        private void DarkButton_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeTheme(App.Theme.Dark);
        }

        private void LightButton_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeTheme(App.Theme.Light);
        }

        private void TransfersButton_Click(object sender, RoutedEventArgs e)
        {
            if (SideView.IsOpen && SideView.PanelContent is QueueView)
            {
                SideView.IsOpen = false;
                return;
            }

            SideView.Title = "Transfers";
            SideView.PanelContent = new QueueView();
            SideView.IsOpen = true;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            if (SideView.IsOpen && SideView.PanelContent is AboutView)
            {
                SideView.IsOpen = false;
                return;
            }

            SideView.Title = "About Dispatch";
            SideView.PanelContent = new AboutView();
            SideView.IsOpen = true;
        }
    }
}
