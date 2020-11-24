using Dispatch.Helpers;
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
                    return RightViewModel.Current?.Name ?? "-";
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
                if (_rightViewModel != null)
                {
                    _rightViewModel.PropertyChanged -= _rightViewModel_PropertyChanged;
                }

                _rightViewModel = value;

                if (_rightViewModel != null)
                {
                    _rightViewModel.PropertyChanged += _rightViewModel_PropertyChanged;
                }

                Notify();
                Notify("Title");
            }
        }

        private void _rightViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Current")
            {
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
