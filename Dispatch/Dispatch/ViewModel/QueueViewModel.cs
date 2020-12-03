using Dispatch.Helpers;

namespace Dispatch.ViewModel
{
    public class QueueViewModel : Observable
    {
        public QueueItem[] Items
        {
            get
            {
                return ResourceQueue.Shared.Items;
            }
        }

        public QueueViewModel()
        {
            ResourceQueue.Shared.OnComplete += Shared_OnComplete;
        }

        private void Shared_OnComplete(object sender, System.EventArgs e)
        {
            Notify("Items");
        }
    }
}
