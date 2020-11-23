using Dispatch.Service.Model;
using Dispatch.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                    var list = sender as ListBox;
                    list.Items.Add(resource);
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var resource = e.AddedItems[0] as Resource;
                ViewModel.Load(resource.Path);

                var list = sender as ListBox;
                list.SelectedItem = null;
            }
        }

        private void ListBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(typeof(Resource)))
            {
                var resource = (Resource)e.Data.GetData(typeof(Resource));

                if (resource.Client == ViewModel.Client)
                {
                    e.Effects = e.AllowedEffects;
                }
            }
        }
    }
}
