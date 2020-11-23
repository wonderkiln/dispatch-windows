using Dispatch.Service.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View
{
    /// <summary>
    /// Interaction logic for ConnectView.xaml
    /// </summary>
    public partial class ConnectView : UserControl
    {
        public event EventHandler<IClient> OnConnected;

        public ConnectView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var client = new LocalClient();

            OnConnected?.Invoke(this, client);
        }
    }
}
