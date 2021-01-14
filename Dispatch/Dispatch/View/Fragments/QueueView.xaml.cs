using Dispatch.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public class QueueItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PendingDataTemplate { get; set; }
        public DataTemplate WorkingDataTemplate { get; set; }
        public DataTemplate DoneDataTemplate { get; set; }
        public DataTemplate ErrorDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var queueItem = (QueueViewModel.QueueItem)item;

            switch (queueItem.Status)
            {
                case QueueViewModel.QueueItem.StatusType.Pending:
                    return PendingDataTemplate;

                case QueueViewModel.QueueItem.StatusType.Working:
                    return WorkingDataTemplate;

                case QueueViewModel.QueueItem.StatusType.Done:
                    return DoneDataTemplate;

                case QueueViewModel.QueueItem.StatusType.Error:
                    return ErrorDataTemplate;

                default:
                    return null;
            }
        }
    }

    public partial class QueueView : UserControl
    {
        public QueueViewModel ViewModel { get; set; }

        public QueueView()
        {
            InitializeComponent();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ClearCompleted();
        }
    }
}
