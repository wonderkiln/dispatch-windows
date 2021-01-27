using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Model;
using Dispatch.View.Fragments;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;
using System.Windows.Media;

namespace Dispatch.ViewModel
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

        public StorageViewModel<FavoriteItem> FavoritesViewModel { get; } = new StorageViewModel<FavoriteItem>("Favorites.json");

        public FavoriteItem.ConnectionType[] Connections
        {
            get
            {
                return new FavoriteItem.ConnectionType[]
                {
                    FavoriteItem.ConnectionType.Sftp,
                    FavoriteItem.ConnectionType.Ftp,
                };
            }
        }

        private FavoriteItem.ConnectionType connection;
        public FavoriteItem.ConnectionType Connection
        {
            get
            {
                return connection;
            }
            set
            {
                connection = value;
                Notify();

                ConnectInfo = CreateConnectInfo(value);
            }
        }

        private FavoriteItem selectedFavorite;
        public FavoriteItem SelectedFavorite
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

        private object connectInfo;
        public object ConnectInfo
        {
            get
            {
                return connectInfo;
            }
            set
            {
                connectInfo = value;
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

        public RelayCommand<FavoriteItem> ConnectFavoriteCommand { get; private set; }
        public RelayCommand<FavoriteItem> EditFavoriteCommand { get; private set; }
        public RelayCommand<FavoriteItem> DeleteFavoriteCommand { get; private set; }
        public RelayCommand<object> SaveAsFavoriteCommand { get; private set; }
        public RelayCommand<object> ConnectCommand { get; private set; }

        public ConnectViewModel()
        {
            ConnectFavoriteCommand = new RelayCommand<FavoriteItem>(ConnectFavorite);
            EditFavoriteCommand = new RelayCommand<FavoriteItem>(EditFavorite);
            DeleteFavoriteCommand = new RelayCommand<FavoriteItem>(DeleteFavorite);
            SaveAsFavoriteCommand = new RelayCommand<object>(SaveAsFavorite);
            ConnectCommand = new RelayCommand<object>(Connect);
            Connection = FavoriteItem.ConnectionType.Sftp;
        }

        private object CreateConnectInfo(FavoriteItem.ConnectionType type, object value = null)
        {
            switch (type)
            {
                case FavoriteItem.ConnectionType.Sftp:
                    {
                        var connectInfo = new SFTPConnectInfo();

                        if (value != null)
                        {
                            var connectionInfo = JObject.FromObject(value).ToObject<SFTPConnectionInfo>();
                            connectInfo.Address = connectionInfo.Address;
                            connectInfo.Port = connectionInfo.Port;
                            connectInfo.Username = connectionInfo.Username;
                            connectInfo.Password = connectionInfo.Password;
                            connectInfo.Key = connectionInfo.Key;
                            connectInfo.Root = connectionInfo.Root;
                        }

                        return connectInfo;
                    }
                case FavoriteItem.ConnectionType.Ftp:
                    {
                        var connectInfo = new FTPConnectInfo();

                        if (value != null)
                        {
                            var connectionInfo = JObject.FromObject(value).ToObject<FTPConnectionInfo>();
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

        private object CreateConnectionInfo(FavoriteItem.ConnectionType type, object value)
        {
            switch (type)
            {
                case FavoriteItem.ConnectionType.Sftp:
                    {
                        var connectInfo = (SFTPConnectInfo)value;
                        return new SFTPConnectionInfo(connectInfo.Address, connectInfo.Port.Value, connectInfo.Username, connectInfo.Password, connectInfo.Key, connectInfo.Root);
                    }
                case FavoriteItem.ConnectionType.Ftp:
                    {
                        var connectInfo = (FTPConnectInfo)value;
                        return new FTPConnectionInfo(connectInfo.Address, connectInfo.Port.Value, connectInfo.Username, connectInfo.Password, connectInfo.Root);
                    }
                default:
                    return null;
            }
        }

        private void ConnectFavorite(FavoriteItem item)
        {
            EditFavorite(item);
            Connect(item);
        }

        private void EditFavorite(FavoriteItem item)
        {
            connection = item.Connection;
            Notify("Connection");

            SelectedFavorite = item;
            ConnectInfo = CreateConnectInfo(item.Connection, item.ConnectionInfo);
        }

        private void DeleteFavorite(FavoriteItem item)
        {
            FavoritesViewModel.Items.Remove(item);
            SelectedFavorite = null;
        }

        private void SaveAsFavorite(object parameter)
        {
            var connectionInfo = CreateConnectionInfo(Connection, ConnectInfo);

            var item = new FavoriteItem
            {
                Connection = Connection,
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
                switch (Connection)
                {
                    case FavoriteItem.ConnectionType.Sftp:
                        {
                            var connectionInfo = (SFTPConnectionInfo)CreateConnectionInfo(Connection, ConnectInfo);
                            var client = await SFTPClient.Create(connectionInfo);
                            OnConnectedClient?.Invoke(this, new ClientEventArgs()
                            {
                                Icon = null,
                                Title = connectionInfo.ToString(),
                                InitialRoot = connectionInfo.Root,
                                Client = client,
                            });
                            break;
                        }
                    case FavoriteItem.ConnectionType.Ftp:
                        {
                            var connectionInfo = (FTPConnectionInfo)CreateConnectionInfo(Connection, ConnectInfo);
                            var client = await FTPClient.Create(connectionInfo);
                            OnConnectedClient?.Invoke(this, new ClientEventArgs()
                            {
                                Icon = null,
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
