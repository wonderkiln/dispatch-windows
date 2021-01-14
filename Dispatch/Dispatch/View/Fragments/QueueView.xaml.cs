using Dispatch.ViewModel;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class QueueView : UserControl
    {
        public QueueViewModel ViewModel { get; set; }

        public QueueView()
        {
            InitializeComponent();
        }
    }
}
