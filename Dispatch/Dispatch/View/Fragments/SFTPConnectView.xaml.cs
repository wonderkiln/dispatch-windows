using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
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

        private string username;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
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
            = new SFTPConnectInfo() { Address = "127.0.0.1", Username = "Adrian", Password = "root", Root = "/Downloads" };
#else
            = new SFTPConnectInfo();
#endif

        public IConnectView ConnectView { get; set; }

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

            ConnectView.OnConnecting();

            try
            {
                var connectionInfo = new SFTPConnectionInfo(ConnectInfo.Address, ConnectInfo.Port.Value, ConnectInfo.Username, ConnectInfo.Password, ConnectInfo.Key, ConnectInfo.Root);
                var client = await SFTPClient.Create(connectionInfo);
                var args = new ConnectViewArgs() { Client = client, InitialPath = connectionInfo.Root, Name = connectionInfo.ToString() };
                ConnectView.OnSuccess(args);
            }
            catch (Exception ex)
            {
                ConnectView.OnException(ex);
            }
        }

        public void Load(object connectionInfo)
        {
            var info = JObject.FromObject(connectionInfo).ToObject<SFTPConnectionInfo>();
            ConnectInfo.Address = info.Address;
            ConnectInfo.Port = info.Port;
            ConnectInfo.Username = info.Username;
            ConnectInfo.Password = info.Password;
            ConnectInfo.Root = info.Root;
            ConnectInfo.Key = info.Key;
        }

        public object GetConnectionInfo()
        {
            return new SFTPConnectionInfo(ConnectInfo.Address, ConnectInfo.Port.Value, ConnectInfo.Username, ConnectInfo.Password, ConnectInfo.Key, ConnectInfo.Root);
        }
    }
}
