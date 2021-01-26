using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public class FTPConnectInfo : Observable
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

        private int? port = 21;
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
    }

    public partial class FTPConnectView : UserControl, IConnectFragment
    {
        public static FTPConnectInfo ConnectInfo { get; }
#if DEBUG
            = new FTPConnectInfo() { Address = "127.0.0.1", Username = "Adrian", Password = "root", Root = "/Downloads" };
#else
            = new FTPConnectInfo();
#endif

        public IConnectView ConnectView { get; set; }

        public FTPConnectView()
        {
            InitializeComponent();
        }

        public async void Connect()
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

            ConnectView.OnConnecting();

            try
            {
                var connectionInfo = new FTPConnectionInfo(ConnectInfo.Address, ConnectInfo.Port.Value, ConnectInfo.Username, ConnectInfo.Password, ConnectInfo.Root);
                var client = await FTPClient.Create(connectionInfo);
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
            var info = JObject.FromObject(connectionInfo).ToObject<FTPConnectionInfo>();
            ConnectInfo.Address = info.Address;
            ConnectInfo.Port = info.Port;
            ConnectInfo.Username = info.Username;
            ConnectInfo.Password = info.Password;
            ConnectInfo.Root = info.Root;
        }

        public object GetConnectionInfo()
        {
            return new FTPConnectionInfo(ConnectInfo.Address, ConnectInfo.Port.Value, ConnectInfo.Username, ConnectInfo.Password, ConnectInfo.Root);
        }
    }
}
