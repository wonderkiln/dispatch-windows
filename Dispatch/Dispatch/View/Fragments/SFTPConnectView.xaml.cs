using Dispatch.Service.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class SFTPConnectView : UserControl
    {
        public string Address { get; set; } = "test.rebex.net";
        public int? Port { get; set; } = 22;
        public string User { get; set; } = "demo";
        public string Password { get; set; } = "password";
        public string Root { get; set; } = "/";

        public event EventHandler<ConnectV> OnConnected;

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

            try
            {
                var client = await SFTPClient.Create(Address, Port.Value, User, Password);
                OnConnected?.Invoke(this, new ConnectV() { Client = client, InitialPath = Root ?? "/", Name = $"{Address}:{Port}" });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
