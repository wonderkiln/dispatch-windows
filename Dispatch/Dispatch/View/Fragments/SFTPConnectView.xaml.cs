using Dispatch.Service.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class SFTPConnectView : UserControl
    {
        public event EventHandler<ConnectV> OnConnected;

        public SFTPConnectView()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var client = await SFTPClient.Create(Host.Text, int.Parse(Port.Text), Username.Text, Password.Password);
                OnConnected?.Invoke(this, new ConnectV() { Client = client, InitialPath = Root.Text, Name = $"{Host.Text}:{Port.Text}" });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
