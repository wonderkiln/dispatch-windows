﻿using Dispatch.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Fragments
{
    public class ConnectViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SFTPDataTemplate { get; set; }
        public DataTemplate FTPDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SFTPConnectionViewModel) return SFTPDataTemplate;
            if (item is FTPConnectionViewModel) return FTPDataTemplate;
            return null;
        }
    }

    public partial class ConnectView : UserControl
    {
        public ConnectView()
        {
            InitializeComponent();
        }
    }
}
