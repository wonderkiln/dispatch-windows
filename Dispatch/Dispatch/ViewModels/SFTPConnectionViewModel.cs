using Dispatch.Helpers;
using Microsoft.Win32;
using System.IO;

namespace Dispatch.ViewModels
{
    public class SFTPConnectionViewModel : ObservableForm
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

        private int? port;
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

        private bool useKey;
        public bool UseKey
        {
            get
            {
                return useKey;
            }
            set
            {
                useKey = value;
                Notify();
            }
        }

        public RelayCommand BrowseKeyCommand { get; }

        public SFTPConnectionViewModel()
        {
            BrowseKeyCommand = new RelayCommand(BrowseKey);
        }

        private void BrowseKey()
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                Key = dialog.FileName;
            }
        }

        internal override string GetError(string propertyName)
        {
            string errorMessage = null;

            switch (propertyName)
            {
                case "Address":
                    if (string.IsNullOrEmpty(Address))
                        errorMessage = "Address must not be empty";

                    break;

                case "Port":
                    if (!Port.HasValue || Port.Value < 1 || Port.Value > 99999)
                        errorMessage = "Port must be a number between 1 and 99999";

                    break;

                case "Username":
                    if (string.IsNullOrEmpty(Username))
                        errorMessage = "Username must not be empty";

                    break;

                case "Password":
                    if (!useKey && string.IsNullOrEmpty(Password))
                        errorMessage = "Password must not be empty";

                    break;

                case "Key":
                    if (useKey && !File.Exists(Key))
                        errorMessage = "Key file was not found";

                    break;

                case "Root":
                    if (string.IsNullOrEmpty(Root))
                        errorMessage = "Root must not be empty";

                    break;
            }

            return errorMessage;
        }
    }
}
