﻿using Dispatch.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

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

        private void ConnectView_OnConnected(object sender, Client.IClient e)
        {
            ViewModel.RightViewModel = new ListViewModel(e);

            OnConnected?.Invoke(this, ViewModel);
        }
    }
}
