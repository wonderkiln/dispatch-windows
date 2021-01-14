using Dispatch.Helpers;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dispatch.ViewModel
{
    public class QueueViewModel
    {
        public class QueueItem : Observable
        {
            public enum StatusType { Pending, Working, Done, Error }

            private StatusType status = StatusType.Pending;
            public StatusType Status
            {
                get
                {
                    return status;
                }
                set
                {
                    status = value;
                    Notify();
                }
            }

            private double progress = 0;
            public double Progress
            {
                get
                {
                    return progress;
                }
                set
                {
                    progress = value;
                    Notify();
                }
            }

            private string name;
            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                    Notify();
                }
            }

            private string error;
            public string Error
            {
                get
                {
                    return error;
                }
                set
                {
                    error = value;
                    Notify();
                }
            }

            public ResourceQueue.Item Tag { get; set; }

            public RelayCommand CancelCommand { get; set; }

            public QueueViewModel ViewModel { get; private set; }

            public QueueItem(QueueViewModel viewModel)
            {
                ViewModel = viewModel;
                CancelCommand = new RelayCommand(Cancel);
            }

            private void Cancel(object arg)
            {
                ViewModel.Cancel(this);
            }
        }

        public ObservableCollection<QueueItem> Items { get; } = new ObservableCollection<QueueItem>();

        public QueueViewModel()
        {
            foreach (var item in ResourceQueue.Shared.Queue)
            {
                Items.Add(Convert(item));
            }

            ResourceQueue.Shared.OnEnqueue += Shared_OnEnqueue;
            ResourceQueue.Shared.OnStart += Shared_OnStart;
            ResourceQueue.Shared.OnProgress += Shared_OnProgress;
            ResourceQueue.Shared.OnFinish += Shared_OnFinish;
            ResourceQueue.Shared.OnError += Shared_OnError;
        }

        private QueueItem Convert(ResourceQueue.Item e)
        {
            return new QueueItem(this) { Tag = e, Name = e.Source.Name };
        }

        private void Shared_OnEnqueue(object sender, ResourceQueue.Item e)
        {
            Items.Add(Convert(e));
        }

        private void Shared_OnStart(object sender, ResourceQueue.Item e)
        {
            var index = Items.IndexOf(Items.First(i => i.Tag == e));
            var item = Convert(e);
            item.Status = QueueItem.StatusType.Working;
            Items[index] = item;
        }

        private void Shared_OnProgress(object sender, ResourceQueue.ProgressEventArgs e)
        {
            var item = Items.First(i => i.Tag == e.Item);
            item.Progress = e.Progress.TotalProgress;
        }

        private void Shared_OnFinish(object sender, ResourceQueue.Item e)
        {
            var index = Items.IndexOf(Items.First(i => i.Tag == e));
            var item = Convert(e);
            item.Status = QueueItem.StatusType.Done;
            Items[index] = item;
        }

        private void Shared_OnError(object sender, ResourceQueue.ErrorEventArgs e)
        {
            var index = Items.IndexOf(Items.First(i => i.Tag == e.Item));
            var item = Convert(e.Item);
            item.Status = QueueItem.StatusType.Error;
            item.Error = e.ErrorMessage;
            Items[index] = item;
        }

        public void Cancel(QueueItem item)
        {
            switch (item.Status)
            {
                case QueueItem.StatusType.Pending:
                    ResourceQueue.Shared.Cancel(item.Tag);
                    Items.Remove(item);
                    break;

                case QueueItem.StatusType.Working:
                    ResourceQueue.Shared.Cancel(item.Tag);
                    break;

                case QueueItem.StatusType.Done:
                case QueueItem.StatusType.Error:
                    Items.Remove(item);
                    break;
            }
        }
    }
}
