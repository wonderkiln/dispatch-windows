using Dispatch.Service.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Dispatch.Helpers
{
    public class QueueItem
    {
        public enum ItemType { Delete }

        public ItemType Type { get; set; }

        public Resource Source { get; set; }

        public Resource Destination { get; set; }

        public Action<Resource, Resource> OnComplete { get; set; }
    }

    public class ResourceQueue
    {
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
            items.Enqueue(new QueueItem()
            {
                Type = type,
                Source = source,
                Destination = destination,
                OnComplete = onComplete,
            });

            if (!Working)
            {
                Dequeue();
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
                    case QueueItem.ItemType.Delete:
                        await item.Source.Client.Delete(item.Source.Path);

                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
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
