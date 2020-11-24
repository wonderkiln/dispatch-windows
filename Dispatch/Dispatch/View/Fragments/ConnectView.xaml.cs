using Dispatch.Service.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class ConnectView : UserControl
    {
        public event EventHandler<IClient> OnConnected;

        public ConnectView()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new FTPClient(Root.Text);
                await client.Connect(Host.Text, int.Parse(Port.Text), Username.Text, Password.Password);

                OnConnected?.Invoke(this, client);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
