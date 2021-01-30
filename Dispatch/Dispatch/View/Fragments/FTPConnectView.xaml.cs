using Dispatch.Helpers;
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

    public partial class FTPConnectView : UserControl
    {
        public FTPConnectView()
        {
            InitializeComponent();
        }
    }
}
