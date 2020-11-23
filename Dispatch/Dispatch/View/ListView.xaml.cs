using Dispatch.Service.Model;
using Dispatch.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

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
    }
}
