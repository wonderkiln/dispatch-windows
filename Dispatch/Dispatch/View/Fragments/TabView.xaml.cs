using Dispatch.Service.Client;
using Dispatch.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View
{
    public class ViewSelector : DataTemplateSelector
    {
        public DataTemplate ListTemplate { get; set; }
        public DataTemplate ConnectTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ListViewModel)
            {
                return ListTemplate;
            }

            return ConnectTemplate;
        }
    }

    /// <summary>
    /// Interaction logic for TabView.xaml
    /// </summary>
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
