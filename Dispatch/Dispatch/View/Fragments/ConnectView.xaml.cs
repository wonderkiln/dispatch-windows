using Dispatch.Service.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public class ConnectViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FTPDataTemplate { get; set; }
        public DataTemplate SFTPDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var index = item as int?;

            switch (index)
            {
                case 0: return FTPDataTemplate;
                case 1: return SFTPDataTemplate;
                default: return null;
            }
        }
    }

    public class ConnectViewArgs
    {
        public IClient Client { get; set; }

        public string InitialPath { get; set; }

        public string Name { get; set; }
    }

    public interface IConnectView
    {
        void OnBeginConnecting();

        void OnSuccess(ConnectViewArgs e);

        void OnException(Exception ex);
    }

    public partial class ConnectView : UserControl, IConnectView
    {
        public static readonly DependencyProperty IsConnectingProperty = DependencyProperty.Register("IsConnecting", typeof(bool), typeof(ConnectView), new PropertyMetadata(false));

        public bool IsConnecting
        {
            get { return (bool)GetValue(IsConnectingProperty); }
            set { SetValue(IsConnectingProperty, value); }
        }

        public event EventHandler<ConnectViewArgs> OnConnected;

        public ConnectView()
        {
            InitializeComponent();
        }

        public void OnBeginConnecting()
        {
            IsConnecting = true;
        }

        public void OnSuccess(ConnectViewArgs e)
        {
            OnConnected?.Invoke(this, e);
        }

        public void OnException(Exception ex)
        {
            IsConnecting = false;
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
