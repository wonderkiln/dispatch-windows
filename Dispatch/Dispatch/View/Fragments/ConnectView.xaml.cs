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

    public partial class ConnectView : UserControl
    {
        public event EventHandler<ConnectV> OnConnected;

        public ConnectView()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = await FTPClient.Create(Host.Text, int.Parse(Port.Text), Username.Text, Password.Password);

                OnConnected?.Invoke(this, new ConnectV() { Client = client, InitialPath = Root.Text, Name = $"{Host.Text}:{Port.Text}" });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
