using Dispatch.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public class ConnectViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SFTPDataTemplate { get; set; }
        public DataTemplate FTPDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SFTPConnectInfo) return SFTPDataTemplate;
            if (item is FTPConnectInfo) return FTPDataTemplate;
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
