using Dispatch.Helpers;
using Dispatch.Service.Client;
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
    }

    public partial class FTPConnectView : UserControl, IConnectFragment
    {
        public static FTPConnectInfo ConnectInfo { get; }
#if DEBUG
            = new FTPConnectInfo() { Address = "127.0.0.1", User = "Adrian", Password = "root", Root = "/Downloads" };
#else
            = new FTPConnectInfo();
#endif

        public static readonly DependencyProperty ConnectViewProperty = DependencyProperty.Register("ConnectView", typeof(IConnectView), typeof(FTPConnectView));
        public IConnectView ConnectView
        {
            get { return (IConnectView)GetValue(ConnectViewProperty); }
            set { SetValue(ConnectViewProperty, value); }
        }

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

            ConnectView.OnBeginConnecting();

            try
            {
                var client = await FTPClient.Create(ConnectInfo.Address, ConnectInfo.Port.Value, ConnectInfo.User, ConnectInfo.Password);

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
            var connectInfo = JObject.FromObject(connectionInfo).ToObject<FTPConnectInfo>();
            ConnectInfo.Address = connectInfo.Address;
            ConnectInfo.Port = connectInfo.Port;
            ConnectInfo.User = connectInfo.User;
            ConnectInfo.Password = connectInfo.Password;
            ConnectInfo.Root = connectInfo.Root;
        }

        public object GetConnectionInfo()
        {
            return ConnectInfo;
        }
    }
}
