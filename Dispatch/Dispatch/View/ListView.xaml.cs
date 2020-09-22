using Dispatch.Client;
using Dispatch.Helpers;
using Dispatch.Screen;
using Dispatch.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Dispatch.View
{
    /// <summary>
    /// Interaction logic for ListView.xaml
    /// </summary>
    public partial class ListView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(ListViewModel), typeof(ListView));

        public ListViewModel ViewModel
        {
            get
            {
                return (ListViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        public ListView()
        {
            InitializeComponent();
        }

        private async void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var item = (ListViewItem)sender;
                var resource = (IResource)item.DataContext;

                if (resource.Directory)
                {
                    ViewModel.Load(resource.Path);
                }
                else
                {
                    var path = Path.Combine(Path.GetTempPath(), resource.Name);
                    await ViewModel.Client.DownloadFile(resource.Path, path);
                    Process.Start(path);
                }
            }
        }

        private class DragDropResource
        {
            public IResource Resource;
        }

        private void ListViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var item = (ListViewItem)sender;
                var resource = (IResource)item.DataContext;

                DragDrop.DoDragDrop(sender as DependencyObject, new DragDropResource() { Resource = resource }, DragDropEffects.Copy);
            }
        }

        private void List_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragDropResource)))
            {
                var data = (DragDropResource)e.Data.GetData(typeof(DragDropResource));

                App.QueueViewModel.Add(data.Resource, ViewModel.CurrentResource);

                // ViewModel.Refresh();
            }
        }

        private void List_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragDropResource)))
            {
                var data = (DragDropResource)e.Data.GetData(typeof(DragDropResource));

                if (ViewModel.Client == data.Resource.Client)
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void List_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragDropResource)))
            {
                var data = (DragDropResource)e.Data.GetData(typeof(DragDropResource));

                if (ViewModel.Client == data.Resource.Client)
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void ListViewItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (ListViewItem)sender;
            var resource = (IResource)item.DataContext;

            var menu = new ContextMenu();

            if (!resource.Directory)
            {
                var editItem = new MenuItem() { Header = "Edit" };
                editItem.Click += EditItem_Click; ;
                menu.Items.Add(editItem);
            }

            var deleteItem = new MenuItem() { Header = "Delete" };
            deleteItem.Click += DeleteItem_Click;
            menu.Items.Add(deleteItem);

            menu.DataContext = resource;
            menu.PlacementTarget = sender as UIElement;
            menu.IsOpen = true;
        }

        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var resource = (IResource)item.DataContext;

            var path = Path.Combine(Path.GetTempPath(), resource.Name);
            await resource.Client.DownloadFile(resource.Path, path);

            ResourceWatcher.Instance.Watch(resource, path);

            Process.Start(path);
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var resource = (IResource)item.DataContext;

            if (MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                ViewModel.Delete(resource.Path);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GoBack();
        }
    }
}
