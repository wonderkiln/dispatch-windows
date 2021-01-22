using Dispatch.Service.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public class SFTPConnectInfo
    {
        public string Address { get; set; }
        public int? Port { get; set; } = 22;
        public string User { get; set; }
        public string Password { get; set; }
        public string Root { get; set; }
    }

    public partial class SFTPConnectView : UserControl
    {
        public static SFTPConnectInfo ConnectInfo { get; }
#if DEBUG
            = new SFTPConnectInfo() { Address = "127.0.0.1", User = "Adrian", Password = "root", Root = "/Downloads" };
#else
            = new SFTPConnectInfo();
#endif

        public static readonly DependencyProperty ConnectViewProperty = DependencyProperty.Register("ConnectView", typeof(IConnectView), typeof(SFTPConnectView));

        public IConnectView ConnectView
        {
            get { return (IConnectView)GetValue(ConnectViewProperty); }
            set { SetValue(ConnectViewProperty, value); }
        }

        public SFTPConnectView()
        {
            InitializeComponent();
        }

        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            AddressTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            PortTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            UserTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            PasswordTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            RootTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (AddressTextBox.GetBindingExpression(TextBox.TextProperty).HasError ||
                PortTextBox.GetBindingExpression(TextBox.TextProperty).HasError ||
                UserTextBox.GetBindingExpression(TextBox.TextProperty).HasError ||
                PasswordTextBox.GetBindingExpression(TextBox.TextProperty).HasError ||
                RootTextBox.GetBindingExpression(TextBox.TextProperty).HasError)
            {
                return;
            }

            ConnectView.OnBeginConnecting();

            try
            {
                var client = await SFTPClient.Create(ConnectInfo.Address, ConnectInfo.Port.Value, ConnectInfo.User, ConnectInfo.Password);

                var args = new ConnectViewArgs() { Client = client, InitialPath = ConnectInfo.Root ?? "/", Name = $"{ConnectInfo.Address}:{ConnectInfo.Port}" };
                ConnectView.OnSuccess(args);
            }
            catch (Exception ex)
            {
                ConnectView.OnException(ex);
            }
        }
    }
}
