using Dispatch.ViewModel;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class QueueView : UserControl
    {
        public QueueViewModel ViewModel { get; } = new QueueViewModel();

        public QueueView()
        {
            InitializeComponent();
        }
    }
}
