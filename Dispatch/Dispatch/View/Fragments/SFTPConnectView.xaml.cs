using Dispatch.Helpers;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public class ObservableForm : Observable, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, string> errors = new Dictionary<string, string>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors
        {
            get
            {
                return errors.Keys.Count > 0;
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName != null && errors.ContainsKey(propertyName))
                return new List<string>() { errors[propertyName] };

            return null;
        }

        private void SetError(string propertyName, string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (errors.ContainsKey(propertyName))
                {
                    errors.Remove(propertyName);
                }
            }
            else if (errors.ContainsKey(propertyName))
            {
                errors[propertyName] = errorMessage;
            }
            else
            {
                errors.Add(propertyName, errorMessage);
            }
        }

        private void NotifyError(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        internal virtual string GetError(string propertyName)
        {
            return null;
        }

        internal void ValidateProperty(string propertyName)
        {
            SetError(propertyName, GetError(propertyName));
            NotifyError(propertyName);
        }

        public bool Validate()
        {
            foreach (var property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                ValidateProperty(property.Name);
            }

            return !HasErrors;
        }

        internal override void Notify([CallerMemberName] string propertyName = null)
        {
            base.Notify(propertyName);
            ValidateProperty(propertyName);
        }
    }

    public class SFTPConnectInfo : ObservableForm
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

        public SFTPConnectInfo()
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
            }

            return errorMessage;
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
