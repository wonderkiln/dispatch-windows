using Dispatch.Client;
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
                var resource = (Resource)item.DataContext;

                if (resource.Type == ResourceType.Directory)
                {
                    ViewModel.Load(resource.Path);
                }
                else
                {
                    var path = await ViewModel.Client.Download(resource, Path.GetTempPath());
                    Process.Start(path);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Up();
        }

        private void ListViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var item = (ListViewItem)sender;
                var resource = (Resource)item.DataContext;

                var menu = new ContextMenu();

                if (resource.Type == ResourceType.File)
                {
                    var editItem = new MenuItem() { Header = "Edit" };
                    editItem.Click += EditItem_Click;
                    menu.Items.Add(editItem);
                }

                menu.PlacementTarget = item;
                menu.IsOpen = true;
            }
        }

        private Dictionary<string, string> editPaths = new Dictionary<string, string>();

        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var resource = (Resource)item.DataContext;

            var path = await ViewModel.Client.Download(resource, Path.GetTempPath());

            editPaths[path] = resource.Path;

            var watcher = new FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(path);
            watcher.Filter = Path.GetFileName(path);
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += Watcher_Changed;

            Process.Start(path);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var remotePath = editPaths[e.FullPath];
            Console.WriteLine(remotePath);
        }

        private void ListViewItem_Drop(object sender, DragEventArgs e)
        {
        }

        private void ListViewItem_DragOver(object sender, DragEventArgs e)
        {
            var item = (ListViewItem)sender;
            List.SelectedItem = item.DataContext;
        }

        private void List_DragOver(object sender, DragEventArgs e)
        {
        }

        private void List_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var list = (System.Windows.Controls.ListView)sender;

            DataObject data = new DataObject("");
            DragDrop.DoDragDrop(list, data, DragDropEffects.Copy);
        }
    }
}
