using Dispatch.Helpers;
using Dispatch.Service.Client;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Dispatch.View.Fragments
{
    public class SFTPConnectInfo : Observable
    {
        private string address;
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                Notify();
            }
        }

        private int? port = 22;
        public int? Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
                Notify();
            }
        }

        private string user;
        public string User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
                Notify();
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                Notify();
            }
        }

        private string root;
        public string Root
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
                Notify();
            }
        }

        private string key;
        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
                Notify();
            }
        }
    }

    public partial class SFTPConnectView : UserControl, IConnectFragment
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

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                ConnectInfo.Key = dialog.FileName;
            }
        }

        public async void Connect()
        {
            var bindings = new List<BindingExpression>()
            {
                AddressTextBox.GetBindingExpression(TextBox.TextProperty),
                PortTextBox.GetBindingExpression(TextBox.TextProperty),
                UserTextBox.GetBindingExpression(TextBox.TextProperty),
                RootTextBox.GetBindingExpression(TextBox.TextProperty),
            };

            if (PrivateKeyCheckBox.IsChecked == true)
                bindings.Add(KeyTextBox.GetBindingExpression(TextBox.TextProperty));
            else
                bindings.Add(PasswordTextBox.GetBindingExpression(TextBox.TextProperty));

            foreach (var binding in bindings) binding.UpdateSource();

            var hasError = bindings.Aggregate(false, (prev, curr) => prev || curr.HasError);
            if (hasError) return;

            ConnectView.OnBeginConnecting();

            try
            {
                SFTPClient client;

                if (ConnectInfo.Key != null)
                {
                    client = await SFTPClient.CreateWithKey(ConnectInfo.Address, ConnectInfo.Port.Value, ConnectInfo.User, ConnectInfo.Key);
                }
                else
                {
                    client = await SFTPClient.Create(ConnectInfo.Address, ConnectInfo.Port.Value, ConnectInfo.User, ConnectInfo.Password);
                }

                var args = new ConnectViewArgs() { Client = client, InitialPath = ConnectInfo.Root ?? "/", Name = $"{ConnectInfo.Address}:{ConnectInfo.Port}" };
                ConnectView.OnSuccess(args);
            }
            catch (Exception ex)
            {
                ConnectView.OnException(ex);
            }
        }

        public void Load(object connectionInfo)
        {
            var connectInfo = JObject.FromObject(connectionInfo).ToObject<SFTPConnectInfo>();
            ConnectInfo.Address = connectInfo.Address;
            ConnectInfo.Port = connectInfo.Port;
            ConnectInfo.User = connectInfo.User;
            ConnectInfo.Password = connectInfo.Password;
            ConnectInfo.Root = connectInfo.Root;
            ConnectInfo.Key = connectInfo.Key;
        }

        public object GetConnectionInfo()
        {
            return ConnectInfo;
        }
    }
}
