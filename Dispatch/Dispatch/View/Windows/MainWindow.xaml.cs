using Dispatch.Controls;
using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using Dispatch.View.Fragments;
using Dispatch.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Windows
{
    public class UpdateStatusDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NoUpdate { get; set; }
        public DataTemplate NewUpdate { get; set; }
        public DataTemplate DownloadingUpdate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is UpdateViewModel.StatusType)) return null;

            var status = (UpdateViewModel.StatusType)item;

            switch (status)
            {
                case UpdateViewModel.StatusType.None:
                    return NoUpdate;

                case UpdateViewModel.StatusType.UpdateAvailable:
                    return NewUpdate;

                case UpdateViewModel.StatusType.Downloading:
                    return DownloadingUpdate;

                default:
                    return null;
            }
        }
    }

    public partial class MainWindow : Window
    {
        public TabsViewModel ViewModel { get; } = new TabsViewModel();

        public QueueViewModel QueueViewModel { get; } = new QueueViewModel();

        public UpdateViewModel UpdateViewModel { get; } = new UpdateViewModel();

        public FavoritesViewModel FavoritesViewModel { get; } = new FavoritesViewModel();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.LoadWindowSettings(this);
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

        private void TransfersButton_Click(object sender, RoutedEventArgs e)
        {
            if (SideView.IsOpen && SideView.PanelContent is QueueView)
            {
                SideView.IsOpen = false;
                return;
            }

            SideView.Title = "Transfers";
            SideView.PanelContent = new QueueView { ViewModel = QueueViewModel };
            SideView.IsOpen = true;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            if (SideView.IsOpen && SideView.PanelContent is MoreView)
            {
                SideView.IsOpen = false;
                return;
            }

            var view = new MoreView();
            view.CloseSidebar = () => { SideView.IsOpen = false; };
            view.ChangeSidebar = (title, newView) =>
            {
                SideView.Title = title;
                SideView.PanelContent = newView;
            };

            SideView.Title = "More";
            SideView.PanelContent = view;
            SideView.IsOpen = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            WindowHelper.SaveWindowSettings(this);
        }

        private void FavoritesMenu_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ResourceDragData)))
            {
                var data = (ResourceDragData)e.Data.GetData(typeof(ResourceDragData));

                foreach (var resource in data.Resources)
                {
                    if (resource.Type == ResourceType.Directory && resource.Client is LocalClient)
                    {
                        FavoritesViewModel.Items.Add(new FavoriteItem(resource));
                    }
                }
            }
        }

        private void FavoriteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)e.OriginalSource;
            var item = (FavoriteItem)menuItem.DataContext;
            var viewModel = (TabViewModel)TabListBox.SelectedItem;
            viewModel.LeftViewModel.Load(item.Path);
        }

        private void FavoriteDeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var item = (FavoriteItem)menuItem.DataContext;
            FavoritesViewModel.Remove(item);
        }
    }
}
