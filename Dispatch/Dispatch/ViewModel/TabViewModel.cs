﻿using Dispatch.Helpers;
using Dispatch.Service.Client;

namespace Dispatch.ViewModel
{
    public class TabViewModel : Observable
    {
        public string Title
        {
            get
            {
                if (RightViewModel != null)
                {
                    return RightViewModel.Client.Name;
                }

                return "New Connection";
            }
        }

        public ListViewModel LeftViewModel { get; } = new ListViewModel(new LocalClient());

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
