using Dispatch.Helpers;
using Dispatch.Service.Client;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dispatch.ViewModel
{
    public class TabViewModel : Observable
    {
        private ImageSource _icon;
        public ImageSource Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                Notify();
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                Notify();
            }
        }

        public ListViewModel LeftViewModel { get; } = new ListViewModel(new LocalClient(), LocalClient.AllDrivesPathKey, null, "Local");

        private ListViewModel _rightViewModel;
        public ListViewModel RightViewModel
        {
            get
            {
                return _rightViewModel;
            }
            set
            {
                _rightViewModel = value;

                if (_rightViewModel != null)
                {
                    Icon = _rightViewModel.Icon;
                    Title = _rightViewModel.Title;
                }

                Notify();
                Notify("Title");
            }
        }

        public async void Disconnect()
        {
            if (RightViewModel != null)
            {
                await RightViewModel.Client.Diconnect();
                RightViewModel = null;
            }
        }
    }
}
