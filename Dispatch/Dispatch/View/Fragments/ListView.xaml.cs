using Dispatch.Helpers;
using Dispatch.Service.Model;
using Dispatch.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dispatch.View.Fragments
{
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

        private void ListViewItem_MouseDoubleClick(object sender, EventArgs e)
        {
            var item = sender as FrameworkElement;
            var resource = item.DataContext as Resource;

            if (resource.Type != ResourceType.File)
            {
                ViewModel.Load(resource.Path);
            }
        }

        private void ListViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var item = sender as FrameworkElement;
                var resource = item.DataContext as Resource;
                DragDrop.DoDragDrop(item, resource, DragDropEffects.Link);
            }
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Resource)))
            {
                var resource = (Resource)e.Data.GetData(typeof(Resource));

                if (resource.Client == ViewModel.Client)
                {
                    ViewModel.Favorites.Add(new FavoriteItem(resource.Name, resource.Path));
                }
            }
        }

        private void ListBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(typeof(Resource)))
            {
                var resource = (Resource)e.Data.GetData(typeof(Resource));

                if (resource.Client == ViewModel.Client && resource.Type != ResourceType.File)
                {
                    e.Effects = e.AllowedEffects;
                }
            }
        }

        private void ShortcutsMenu_Click(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as FrameworkElement;
            var favorite = item.DataContext as FavoriteItem;
            ViewModel.Load(favorite.Path);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                Keyboard.ClearFocus();
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            var resource = item.DataContext as Resource;

            if (MessageBox.Show($"Are you sure you want to delete '{resource.Name}'?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ResourceQueue.Shared.Add(QueueItem.ItemType.Delete, resource, null, (source, destination) =>
                {
                    ViewModel.Refresh();
                });
            }
        }

        private void DeleteFavoriteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            var favorite = item.DataContext as FavoriteItem;

            ViewModel.Favorites.Remove(favorite);
        }

        public event EventHandler<Resource> BeginUpload;

        private void UploadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (Resource resource in List.SelectedItems)
            {
                BeginUpload?.Invoke(this, resource);
            }
        }
    }
}
