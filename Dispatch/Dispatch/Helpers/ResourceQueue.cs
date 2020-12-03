using Dispatch.Service.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace Dispatch.Helpers
{
    public class QueueItem : Observable
    {
        public enum ItemType { Upload, Delete }

        public ItemType Type { get; set; }

        public Resource Source { get; set; }

        public Resource Destination { get; set; }

        public Action<Resource, Resource> OnComplete { get; set; }

        public ProgressStatus Progress { get; set; }

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public CancellationToken Token
        {
            get
            {
                return tokenSource.Token;
            }
        }

        public RelayCommand CancelCommand { get; private set; }

        public QueueItem()
        {
            CancelCommand = new RelayCommand(CancelCommandAction);
        }

        private void CancelCommandAction(object parameters)
        {
            tokenSource.Cancel();
        }
    }

    public class ResourceQueue
    {
        public event EventHandler<QueueItem> OnAddedItem;
        public event EventHandler<QueueItem> OnFinishedItem;

        public static ResourceQueue Shared = new ResourceQueue();

        private readonly Queue<QueueItem> items = new Queue<QueueItem>();

        public bool Working { get; private set; } = false;

        public QueueItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        public void Add(QueueItem.ItemType type, Resource source, Resource destination, Action<Resource, Resource> onComplete)
        {
            var item = new QueueItem()
            {
                Type = type,
                Source = source,
                Destination = destination,
                OnComplete = onComplete,
            };

            items.Enqueue(item);

            OnAddedItem?.Invoke(this, item);

            if (!Working)
            {
                Dequeue();
            }
        }

        private class XXX : IProgress<ProgressStatus>
        {
            private readonly QueueItem item;

            public XXX(QueueItem item)
            {
                this.item = item;
            }

            public void Report(ProgressStatus value)
            {
                item.Progress = value;
                item.Notify("Progress");
            }
        }

        private async void Dequeue()
        {
            if (items.Count == 0 || Working)
            {
                return;
            }

            Working = true;

            var item = items.Dequeue();

            try
            {
                switch (item.Type)
                {
                    case QueueItem.ItemType.Upload:
                        // Create separate session for upload
                        var client1 = await item.Destination.Client.Clone();
                        await client1.Upload(item.Destination.Path, item.Source.Path, new XXX(item), item.Token);

                        break;

                    case QueueItem.ItemType.Delete:
                        // Create separate session for deletion
                        var client2 = await item.Source.Client.Clone();
                        await client2.Delete(item.Source.Path);

                        break;
                }
            }
            catch (Exception ex)
            {
                // TODO:
                Console.WriteLine(ex);
            }
            finally
            {
                OnFinishedItem?.Invoke(this, item);

                if (item.OnComplete != null)
                {
                    item.OnComplete(item.Source, item.Destination);
                }
            }

            Working = false;

            Dequeue();
        }
    }
}
