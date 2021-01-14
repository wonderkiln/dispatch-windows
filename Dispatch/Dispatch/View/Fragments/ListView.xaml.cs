﻿using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using Dispatch.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            if (MessageBox.Show($"Are you sure you want to delete {List.SelectedItems.Count} item(s)?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var resources = new List<Resource>(List.SelectedItems.Cast<Resource>());

                foreach (var resource in resources)
                {
                    //ResourceQueue.Shared.Add(QueueItem.ItemType.Delete, resource, null, (source, destination) =>
                    //{
                    //    ViewModel.Refresh();
                    //});
                }
            }
        }

        public event EventHandler<Resource> BeginUpload;

        private void UploadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (Resource resource in List.SelectedItems)
            {
                BeginUpload?.Invoke(this, resource);
            }
        }

        private void List_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            OpenMenuItem.Visibility = List.SelectedItems.Count == 1 ? Visibility.Visible : Visibility.Collapsed;
            EditMenuItem.Visibility = !(ViewModel.Client is LocalClient) && List.SelectedItems.Count == 1 ? Visibility.Visible : Visibility.Collapsed;
            UploadMenuItem.Visibility = ViewModel.Client is LocalClient && List.SelectedItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            DeleteMenuItem.Visibility = List.SelectedItems.Count > 0 && ((Resource)List.SelectedItem).Type != ResourceType.Drive ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var resource = List.SelectedItem as Resource;

            if (resource.Type != ResourceType.File)
            {
                ViewModel.Load(resource.Path);
            }
            else
            {
                if (ViewModel.Client is LocalClient)
                {
                    Process.Start(resource.Path);
                }
                else
                {
                    var destination = new Resource(null, Path.GetTempPath(), "");

                    //ResourceQueue.Shared.Add(QueueItem.ItemType.Download, resource, destination, (source, destination2) =>
                    //{
                    //    Process.Start(Path.Combine(destination2.Path, resource.Name));
                    //});
                }
            }
        }

        private void List_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(typeof(Resource)))
            {
                var resource = (Resource)e.Data.GetData(typeof(Resource));

                if (resource.Client != ViewModel.Client && resource.Type != ResourceType.Drive)
                {
                    e.Effects = e.AllowedEffects;
                }
            }
        }

        private void List_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Resource)))
            {
                var resource = (Resource)e.Data.GetData(typeof(Resource));

                if (ViewModel.Client is LocalClient && !(resource.Client is LocalClient))
                {
                    ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Download, resource, new Resource(ViewModel.Client, ViewModel.CurrentPath, "")));
                }
                else if (!(ViewModel.Client is LocalClient) && resource.Client is LocalClient)
                {
                    ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Upload, resource, new Resource(ViewModel.Client, ViewModel.CurrentPath, "")));
                }
            }
        }
    }
}
