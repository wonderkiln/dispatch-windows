using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;
using System.Windows.Media;

namespace Dispatch.ViewModels
{
    public class ConnectViewModel : Observable
    {
        public class ClientEventArgs : EventArgs
        {
            public ImageSource Icon { get; set; }

            public string Title { get; set; }

            public string InitialRoot { get; set; }

            public IClient Client { get; set; }
        }


        public event EventHandler<ClientEventArgs> OnConnectedClient;

        public StorageViewModel<Favorite> FavoritesViewModel { get; } = new StorageViewModel<Favorite>("Favorites.json");

        public Favorite.ConnectionType[] ConnectionTypes
        {
            get
            {
                return new Favorite.ConnectionType[]
                {
                    Favorite.ConnectionType.Sftp,
                    Favorite.ConnectionType.Ftp,
                };
            }
        }

        private Favorite.ConnectionType connectionType;
        public Favorite.ConnectionType ConnectionType
        {
            get
            {
                return connectionType;
            }
            set
            {
                connectionType = value;
                Notify();

                Connection = CreateConnectInfo(value);
            }
        }

        private Favorite selectedFavorite;
        public Favorite SelectedFavorite
        {
            get
            {
                return selectedFavorite;
            }
            set
            {
                selectedFavorite = value;
                Notify();
            }
        }

        private ObservableForm connection;
        public ObservableForm Connection
        {
            get
            {
                return connection;
            }
            set
            {
                connection = value;
                Notify();

                SaveAsFavoriteCommand.IsExecutable = value != null;
                ConnectCommand.IsExecutable = value != null;
            }
        }

        private bool isConnecting = false;
        public bool IsConnecting
        {
            get
            {
                return isConnecting;
            }
            set
            {
                isConnecting = value;
                Notify();
            }
        }

        public RelayCommand<Favorite> ConnectFavoriteCommand { get; private set; }
        public RelayCommand<Favorite> EditFavoriteCommand { get; private set; }
        public RelayCommand<Favorite> DeleteFavoriteCommand { get; private set; }
        public RelayCommand<object> SaveAsFavoriteCommand { get; private set; }
        public RelayCommand<object> ConnectCommand { get; private set; }

        public ConnectViewModel()
        {
            ConnectFavoriteCommand = new RelayCommand<Favorite>(ConnectFavorite);
            EditFavoriteCommand = new RelayCommand<Favorite>(EditFavorite);
            DeleteFavoriteCommand = new RelayCommand<Favorite>(DeleteFavorite);
            SaveAsFavoriteCommand = new RelayCommand<object>(SaveAsFavorite);
            ConnectCommand = new RelayCommand<object>(Connect);
            ConnectionType = Favorite.ConnectionType.Sftp;
        }

        private ObservableForm CreateConnectInfo(Favorite.ConnectionType type, object value = null)
        {
            switch (type)
            {
                case Favorite.ConnectionType.Sftp:
                    {
                        var connectInfo = new SFTPConnectionViewModel();

                        if (value != null)
                        {
                            var connectionInfo = JObject.FromObject(value).ToObject<SFTPConnection>();
                            connectInfo.Address = connectionInfo.Address;
                            connectInfo.Port = connectionInfo.Port;
                            connectInfo.Username = connectionInfo.Username;
                            connectInfo.Password = connectionInfo.Password;
                            connectInfo.Key = connectionInfo.Key;
                            connectInfo.Root = connectionInfo.Root;
                        }

                        return connectInfo;
                    }
                case Favorite.ConnectionType.Ftp:
                    {
                        var connectInfo = new FTPConnectionViewModel();

                        if (value != null)
                        {
                            var connectionInfo = JObject.FromObject(value).ToObject<FTPConnection>();
                            connectInfo.Address = connectionInfo.Address;
                            connectInfo.Port = connectionInfo.Port;
                            connectInfo.Username = connectionInfo.Username;
                            connectInfo.Password = connectionInfo.Password;
                            connectInfo.Root = connectionInfo.Root;
                        }

                        return connectInfo;
                    }
                default:
                    return null;
            }
        }

        private object CreateConnectionInfo(Favorite.ConnectionType type, object value)
        {
            switch (type)
            {
                case Favorite.ConnectionType.Sftp:
                    {
                        var connectInfo = (SFTPConnectionViewModel)value;
                        if (!connectInfo.Validate()) return null;
                        return new SFTPConnection(connectInfo.Address, connectInfo.Port.Value, connectInfo.Username, connectInfo.Password, connectInfo.Key, connectInfo.Root);
                    }
                case Favorite.ConnectionType.Ftp:
                    {
                        var connectInfo = (FTPConnectionViewModel)value;
                        if (!connectInfo.Validate()) return null;
                        return new FTPConnection(connectInfo.Address, connectInfo.Port.Value, connectInfo.Username, connectInfo.Password, connectInfo.Root);
                    }
                default:
                    return null;
            }
        }

        private void ConnectFavorite(Favorite item)
        {
            EditFavorite(item);
            Connect(item);
        }

        private void EditFavorite(Favorite item)
        {
            connectionType = item.Connection;
            Notify("Connection");

            SelectedFavorite = item;
            Connection = CreateConnectInfo(item.Connection, item.ConnectionInfo);
        }

        private void DeleteFavorite(Favorite item)
        {
            if (MessageBox.Show($"Are you sure you want to delete '{item.Title}'?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                FavoritesViewModel.Items.Remove(item);
                SelectedFavorite = null;
            }
        }

        private void SaveAsFavorite(object parameter)
        {
            var connectionInfo = CreateConnectionInfo(ConnectionType, Connection);
            if (connectionInfo == null) return;

            var item = new Favorite
            {
                Connection = ConnectionType,
                Title = connectionInfo.ToString(),
                ConnectionInfo = connectionInfo,
            };

            FavoritesViewModel.Items.Add(item);
        }

        private void ShowErrorMessage(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void Connect(object parameter)
        {
            IsConnecting = true;

            try
            {
                switch (ConnectionType)
                {
                    case Favorite.ConnectionType.Sftp:
                        {
                            if (!Connection.Validate()) return;
                            var connectionInfo = (SFTPConnection)CreateConnectionInfo(ConnectionType, Connection);
                            var client = await SFTPClient.Create(connectionInfo);
                            OnConnectedClient?.Invoke(this, new ClientEventArgs()
                            {
                                Icon = Icons.Sftp,
                                Title = connectionInfo.ToString(),
                                InitialRoot = connectionInfo.Root,
                                Client = client,
                            });
                            break;
                        }
                    case Favorite.ConnectionType.Ftp:
                        {
                            var connectionInfo = (FTPConnection)CreateConnectionInfo(ConnectionType, Connection);
                            if (!Connection.Validate()) return;
                            var client = await FTPClient.Create(connectionInfo);
                            OnConnectedClient?.Invoke(this, new ClientEventArgs()
                            {
                                Icon = Icons.Ftp,
                                Title = connectionInfo.ToString(),
                                InitialRoot = connectionInfo.Root,
                                Client = client,
                            });
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
            finally
            {
                IsConnecting = false;
            }
        }
    }
}
