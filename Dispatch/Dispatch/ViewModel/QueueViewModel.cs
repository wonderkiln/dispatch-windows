using Dispatch.Helpers;
using System.Collections.ObjectModel;

namespace Dispatch.ViewModel
{
    public class QueueViewModel : Observable
    {
        public ObservableCollection<QueueItem> Items { get; } = new ObservableCollection<QueueItem>();

        public QueueViewModel()
        {
            ResourceQueue.Shared.OnAddedItem += Shared_OnAddedItem;
            ResourceQueue.Shared.OnFinishedItem += Shared_OnFinishedItem;
        }

        private void Shared_OnAddedItem(object sender, QueueItem e)
        {
            Items.Add(e);
        }

        private void Shared_OnFinishedItem(object sender, QueueItem e)
        {
            Items.Remove(e);
        }
    }
}
