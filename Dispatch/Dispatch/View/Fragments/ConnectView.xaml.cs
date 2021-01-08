using Dispatch.Service.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public class ConnectV
    {
        public IClient Client { get; set; }

        public string InitialPath { get; set; }

        public string Name { get; set; }
    }

    public class ConnectViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FTPDataTemplate { get; set; }
        public DataTemplate SFTPDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var index = (int)item;

            switch (index)
            {
                case 0: return FTPDataTemplate;
                case 1: return SFTPDataTemplate;
                default: return null;
            }
        }
    }

    public partial class ConnectView : UserControl
    {
        public event EventHandler<ConnectV> OnConnected;

        public ConnectView()
        {
            InitializeComponent();
        }

        private void ConnectView_OnConnected(object sender, ConnectV e)
        {
            OnConnected?.Invoke(this, e);
        }
    }
}
