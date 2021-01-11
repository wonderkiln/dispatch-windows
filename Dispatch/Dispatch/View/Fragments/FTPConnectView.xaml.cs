using Dispatch.Service.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class FTPConnectView : UserControl
    {
        public string Address { get; set; } = "waws-prod-dm1-173.ftp.azurewebsites.windows.net";
        public int? Port { get; set; } = 21;
        public string User { get; set; } = "$app-dispatchftp-dev";
        public string Password { get; set; } = "w2yZ0XW2PvDtDrnvzcyS6Zmj6B9uMxEvCkdP5Jo1jxgxKH7h9uYadcmEivlx";
        public string Root { get; set; } = "/site";

        public event EventHandler<ConnectV> OnConnected;

        public FTPConnectView()
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

            try
            {
                var client = await FTPClient.Create(Address, Port.Value, User, Password);
                OnConnected?.Invoke(this, new ConnectV() { Client = client, InitialPath = Root ?? "/", Name = $"{Address}:{Port}" });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
