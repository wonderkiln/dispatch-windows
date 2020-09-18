using Dispatch.Client;
using Dispatch.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    return RightViewModel.ToString();
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
    }
}
