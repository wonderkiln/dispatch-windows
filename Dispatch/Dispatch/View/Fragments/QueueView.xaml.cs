using Dispatch.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class QueueView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(QueueViewModel), typeof(QueueView));

        public QueueViewModel ViewModel
        {
            get
            {
                return (QueueViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        public QueueView()
        {
            InitializeComponent();
        }
    }
}
