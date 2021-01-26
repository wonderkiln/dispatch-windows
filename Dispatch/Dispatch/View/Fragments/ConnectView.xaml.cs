using Dispatch.Service.Client;
using Dispatch.Service.Model;
using Dispatch.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Dispatch.View.Fragments
{
    public enum ConnectViewType { Sftp, Ftp }

    public class ConnectViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FTPDataTemplate { get; set; }
        public DataTemplate SFTPDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is ConnectViewType)) return null;

            var type = (ConnectViewType)item;

            switch (type)
            {
                case ConnectViewType.Sftp: return SFTPDataTemplate;
                case ConnectViewType.Ftp: return FTPDataTemplate;
                default: return null;
            }
        }
    }

    public class ConnectViewArgs
    {
        public IClient Client { get; set; }

        public string InitialPath { get; set; }

        public string Name { get; set; }
    }

    public interface IConnectView
    {
        void OnConnecting();

        void OnSuccess(ConnectViewArgs e);

        void OnException(Exception ex);
    }

    public interface IConnectFragment
    {
        void Connect();
        void Load(object connectionInfo);
        object GetConnectionInfo();
    }

    public partial class ConnectView : UserControl, IConnectView
    {
        public StorageViewModel<FavoriteItem> FavoritesViewModel { get; } = new StorageViewModel<FavoriteItem>("Favorites.json");

        public static readonly DependencyProperty IsConnectingProperty = DependencyProperty.Register("IsConnecting", typeof(bool), typeof(ConnectView), new PropertyMetadata(false));
        public bool IsConnecting
        {
            get { return (bool)GetValue(IsConnectingProperty); }
            set { SetValue(IsConnectingProperty, value); }
        }

        public event EventHandler<ConnectViewArgs> OnConnected;

        public ConnectView()
        {
            InitializeComponent();
            ComboBox.SelectedItem = ConnectViewType.Sftp;
        }

        public void OnConnecting()
        {
            IsConnecting = true;
        }

        public void OnSuccess(ConnectViewArgs e)
        {
            IsConnecting = false;
            OnConnected?.Invoke(this, e);
        }

        public void OnException(Exception ex)
        {
            IsConnecting = false;
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var content = (IConnectFragment)VisualTreeHelper.GetChild(Presenter, 0);
            content.Connect();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var content = (IConnectFragment)VisualTreeHelper.GetChild(Presenter, 0);
            var connectionInfo = content.GetConnectionInfo();
            var type = (ConnectViewType)ComboBox.SelectedItem;
            FavoritesViewModel.Items.Add(new FavoriteItem() { Title = "Server", Type = type, ConnectionInfo = connectionInfo });
        }

        private void LoadFavorite(FavoriteItem item, bool autoConnect)
        {
            ComboBox.SelectedItem = item.Type;

            var content = (IConnectFragment)Presenter.Content;
            content.Load(item.ConnectionInfo);

            if (autoConnect)
            {
                content.Connect();
            }
        }

        private void FavoriteListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listItem = (ListBoxItem)sender;
            var item = (FavoriteItem)listItem.DataContext;
            LoadFavorite(item, true);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var type = (ConnectViewType)e.AddedItems[0];

                switch (type)
                {
                    case ConnectViewType.Sftp:
                        Presenter.Content = new SFTPConnectView() { ConnectView = this };
                        break;
                    case ConnectViewType.Ftp:
                        Presenter.Content = new FTPConnectView() { ConnectView = this };
                        break;
                    default:
                        break;
                }
            }
        }

        private void FavoritesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = (FavoriteItem)e.AddedItems[0];
                LoadFavorite(item, false);
            }
        }

        private void FavoriteDeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var item = (FavoriteItem)menuItem.DataContext;
            FavoritesViewModel.Items.Remove(item);
        }
    }
}
