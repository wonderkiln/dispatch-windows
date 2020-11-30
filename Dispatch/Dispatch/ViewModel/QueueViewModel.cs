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
    }
}
