using Dispatch.Helpers;
using Microsoft.Win32;
using System.Windows.Controls;

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

        public RelayCommand<object> BrowseKeyCommand { get; private set; }

        public SFTPConnectInfo()
        {
            BrowseKeyCommand = new RelayCommand<object>(BrowseKey);
        }

        private void BrowseKey(object parameter)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                Key = dialog.FileName;
            }
        }
    }

    public partial class SFTPConnectView : UserControl
    {
        public SFTPConnectView()
        {
            InitializeComponent();
        }
    }
}
