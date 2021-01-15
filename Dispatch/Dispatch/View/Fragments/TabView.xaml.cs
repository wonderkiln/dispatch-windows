﻿using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using Dispatch.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        private void ConnectView_OnConnected(object sender, ConnectViewArgs e)
        {
            ImageSource source = null;

            if (e.Client is FTPClient)
            {
                source = new BitmapImage(new Uri("/Resources/ic_ftp.png", UriKind.Relative));
            }
            else if (e.Client is SFTPClient)
            {
                source = new BitmapImage(new Uri("/Resources/ic_sftp.png", UriKind.Relative));
            }

            ViewModel.RightViewModel = new ListViewModel(e.Client, e.InitialPath, source, e.Name);

            OnConnected?.Invoke(this, ViewModel);
        }

        private void ResourceView_BeginTransfer(object sender, Resource[] e)
        {
            foreach (var resource in e)
            {
                // LocalClient is always on the left side
                if (resource.Client is LocalClient)
                {
                    ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Upload, resource, new Resource(ViewModel.RightViewModel.Client, ViewModel.RightViewModel.CurrentPath, ""), ViewModel.RightViewModel));
                }
                else
                {
                    ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Download, resource, new Resource(ViewModel.LeftViewModel.Client, ViewModel.LeftViewModel.CurrentPath, ""), ViewModel.LeftViewModel));
                }
            }
        }
    }
}
