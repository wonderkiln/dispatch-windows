using Dispatch.Client;
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

        ProgressWindow progressWindow;

        private async void List_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragDropResource)))
            {
                var data = (DragDropResource)e.Data.GetData(typeof(DragDropResource));

                var draggedResource = data.Resource;
                var droppedResource = ViewModel.CurrentResource;

                var draggedClient = draggedResource.Client;
                var droppedClient = ViewModel.Client;

                progressWindow = new ProgressWindow();
                progressWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                progressWindow.Owner = Parent as Window;
                progressWindow.Show();

                if (draggedClient is LocalClient)
                {
                    droppedClient.OnProgressChange += DroppedClient_OnProgressChange;

                    if (draggedResource.Directory)
                    {
                        await droppedClient.UploadDirectory(draggedResource.Path, droppedResource.CombinePath(draggedResource.Name));
                    }
                    else
                    {
                        await droppedClient.UploadFile(draggedResource.Path, droppedResource.CombinePath(draggedResource.Name));
                    }

                    droppedClient.OnProgressChange -= DroppedClient_OnProgressChange;
                }
                else
                {
                    draggedClient.OnProgressChange += DroppedClient_OnProgressChange;

                    if (draggedResource.Directory)
                    {
                        await draggedClient.DownloadDirectory(draggedResource.Path, droppedResource.CombinePath(draggedResource.Name));
                    }
                    else
                    {
                        await draggedClient.DownloadFile(draggedResource.Path, droppedResource.CombinePath(draggedResource.Name));
                    }

                    draggedClient.OnProgressChange -= DroppedClient_OnProgressChange;
                }

                progressWindow.Close();
                progressWindow = null;

                ViewModel.Refresh();
            }
        }

        private void DroppedClient_OnProgressChange(object sender, ClientProgress e)
        {
            progressWindow.Progress.IsIndeterminate = false;
            progressWindow.Progress.Value = e.TotalProgress;
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
    }
}
