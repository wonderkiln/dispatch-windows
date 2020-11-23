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
                    return RightViewModel.Current?.Path ?? "-";
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
                _rightViewModel.PropertyChanged += _rightViewModel_PropertyChanged;

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
    }
}
