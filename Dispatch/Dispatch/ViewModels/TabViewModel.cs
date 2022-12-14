using Dispatch.Helpers;
using Dispatch.Service.Client;
using Dispatch.Service.Models;
using System;
using System.Windows.Media;

namespace Dispatch.ViewModels
{
    public class TabViewModel : Observable
    {
        private ImageSource icon;
        public ImageSource Icon
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
                Notify();
            }
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                Notify();
            }
        }

        private object leftSide;
        public object LeftSide
        {
            get
            {
                return leftSide;
            }
            set
            {
                leftSide = value;
                Notify();

                if (value is ResourcesViewModel vm)
                {
                    vm.OnAddBookmark += ResourcesViewModel_OnAddBookmark;
                }
            }
        }

        private object rightSide;
        public object RightSide
        {
            get
            {
                return rightSide;
            }
            set
            {
                rightSide = value;
                Notify();

                if (value is ResourcesViewModel vm)
                {
                    vm.OnAddBookmark += ResourcesViewModel_OnAddBookmark;
                }
            }
        }

        public RelayCommand<Bookmark> NavigateToBookmarkCommand { get; }

        public event EventHandler<Resource[]> OnAddBookmark;

        public TabViewModel()
        {
            NavigateToBookmarkCommand = new RelayCommand<Bookmark>(NavigateToBookmark);

            LeftSide = new ResourcesViewModel(new LocalClient(), LocalClient.AllDrivesPathKey);

            var connectViewModel = new ConnectViewModel();
            connectViewModel.OnConnectedClient += ConnectViewModel_OnConnectedClient;
            RightSide = connectViewModel;

            Icon = Icons.Bolt;
            Title = "Quick Connect";
        }

        private void ConnectViewModel_OnConnectedClient(object sender, ConnectViewModel.ClientEventArgs e)
        {
            var rightSide = new ResourcesViewModel(e.Client, e.InitialRoot);

            if (LeftSide is ResourcesViewModel leftSide)
            {
                rightSide.Side = leftSide;
                leftSide.Side = rightSide;
            }

            RightSide = rightSide;
            Icon = e.Icon;
            Title = e.Title;
        }

        private void ResourcesViewModel_OnAddBookmark(object sender, Resource[] e)
        {
            OnAddBookmark?.Invoke(this, e);
        }

        private void NavigateToBookmark(Bookmark item)
        {
            if (item.IsLocal)
            {
                if (leftSide is ResourcesViewModel vm)
                {
                    vm.DisplayPath = item.Path;
                }
            }
            else
            {
                if (rightSide is ResourcesViewModel vm)
                {
                    vm.DisplayPath = item.Path;
                }
            }
        }

        public async void Disconnect()
        {
            if (leftSide is ResourcesViewModel leftVM)
            {
                await leftVM.Disconnect();
            }

            if (rightSide is ResourcesViewModel rightVM)
            {
                await rightVM.Disconnect();
            }
        }
    }
}
