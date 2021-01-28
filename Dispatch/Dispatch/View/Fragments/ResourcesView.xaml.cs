using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using Dispatch.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dispatch.View.Fragments
{
    public class ResourceDragData
    {
        public Resource[] Resources { get; private set; }

        public Resource Resource
        {
            get
            {
                return Resources.First();
            }
        }

        public bool IsSingle
        {
            get
            {
                return Resources.Length == 1;
            }
        }

        public bool HasData
        {
            get
            {
                return Resources.Length > 0;
            }
        }

        public ResourceDragData(Resource resource)
        {
            Resources = new Resource[] { resource };
        }

        public ResourceDragData(IEnumerable<Resource> resource)
        {
            Resources = resource.ToArray();
        }
    }

    public partial class ResourcesView : UserControl, IContextServiceActions
    {
        public event EventHandler<Resource[]> BeginTransfer;

        public ResourcesView()
        {
            InitializeComponent();
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

        private void List_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(typeof(ResourceDragData)))
            {
                var data = (ResourceDragData)e.Data.GetData(typeof(ResourceDragData));

                //if (data.HasData && data.Resource.Client != ViewModel.Client && data.Resource.Type != ResourceType.Drive)
                //{
                //    e.Effects = e.AllowedEffects;
                //}
            }
        }

        private void List_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ResourceDragData)))
            {
                var data = (ResourceDragData)e.Data.GetData(typeof(ResourceDragData));

                foreach (var resource in data.Resources)
                {
                    //if (ViewModel.Client is LocalClient && !(resource.Client is LocalClient))
                    //{
                    //    ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Download, resource, new Resource(ViewModel.Client, ViewModel.CurrentPath, ""), ViewModel));
                    //}
                    //else if (!(ViewModel.Client is LocalClient) && resource.Client is LocalClient)
                    //{
                    //    ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Upload, resource, new Resource(ViewModel.Client, ViewModel.CurrentPath, ""), ViewModel));
                    //}
                }
            }
        }

        private Resource[] selectedItems = new Resource[] { };
        private Point startDragPosition;

        private void List_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && startDragPosition != null)
            {
                var position = e.GetPosition((IInputElement)sender);
                var distance = Point.Subtract(position, startDragPosition).Length;

                if (Math.Abs(distance) > 10)
                {
                    var listView = sender as ListView;
                    var selectedItem = listView.SelectedItem as Resource;

                    if (selectedItems.Contains(selectedItem))
                    {
                        listView.SelectedItems.Clear();

                        foreach (var item in selectedItems)
                        {
                            listView.SelectedItems.Add(item);
                        }
                    }
                    else if (selectedItem != null)
                    {
                        selectedItems = new Resource[] { selectedItem };
                    }

                    if (selectedItems.Length > 0)
                    {
                        var data = new ResourceDragData(selectedItems);
                        DragDrop.DoDragDrop(listView, data, DragDropEffects.Link);
                    }
                }
            }
        }

        private void List_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var listView = sender as ListView;
                selectedItems = listView.SelectedItems.Cast<Resource>().ToArray();
                startDragPosition = e.GetPosition(listView);
            }
        }

        private void List_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            var resources = listView.SelectedItems.Cast<Resource>().ToArray();
            //ContextService.Show(resources, listView, this);
        }

        public void ContextServiceOpen(Resource resource)
        {
            if (resource.Type != ResourceType.File)
            {
                //ViewModel.Load(resource.Path);
            }
        }

        public void ContextServiceTransfer(Resource[] resources)
        {
            BeginTransfer?.Invoke(this, resources);
        }

        public void ContextServiceDelete(Resource[] resources)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected items?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var resource in resources)
                {
                    //ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Delete, resource, null, ViewModel));
                }
            }
        }
    }
}
