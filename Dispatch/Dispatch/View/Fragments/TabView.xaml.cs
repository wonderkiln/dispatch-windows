using Dispatch.Service.Client;
using Dispatch.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class TabView : UserControl
    {
        public event EventHandler<TabViewModel> OnConnected;

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(TabViewModel), typeof(TabView));

        public TabViewModel ViewModel
        {
            get
            {
                return (TabViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        public TabView()
        {
            InitializeComponent();
        }

        private void ConnectView_OnConnected(object sender, IClient e)
        {
            ViewModel.RightViewModel = new ListViewModel(e);

            OnConnected?.Invoke(this, ViewModel);
        }
    }
}
