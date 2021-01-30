using Dispatch.Service.Model;
using Dispatch.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Dispatch.Helpers
{
    public class ResourceQueue
    {
        public class Item
        {
            public enum ActionType { Upload, Download, Delete }

            public ActionType Action { get; private set; }

            public Resource Source { get; private set; }

            public Resource Destination { get; private set; }

            public ResourcesViewModel ViewModel { get; private set; }

            public Item(ActionType action, Resource source, Resource destination, ResourcesViewModel viewModel)
            {
                Action = action;
                Source = source;
                Destination = destination;
                ViewModel = viewModel;
            }
        }

        public class ProgressEventArgs
        {
            public Item Item { get; private set; }

            public ProgressStatus Progress { get; private set; }

            public ProgressEventArgs(Item item, ProgressStatus progress)
            {
                Item = item;
                Progress = progress;
            }
        }

        public class ErrorEventArgs
        {
            public Item Item { get; private set; }

            public Exception Error { get; private set; }

            public string ErrorMessage
            {
                get
                {
                    return Error.Message;
                }
            }

            public ErrorEventArgs(Item item, Exception error)
            {
                Item = item;
                Error = error;
            }
        }

        private class WorkerProgressReporter : System.IProgress<ProgressStatus>
        {
            public Item Item { get; private set; }

            public ResourceQueue Queue { get; private set; }

            public WorkerProgressReporter(Item item, ResourceQueue queue)
            {
                Item = item;
                Queue = queue;
            }

            public void Report(ProgressStatus value)
            {
                var args = new ProgressEventArgs(Item, value);

                Queue.dispatcher.Invoke(() =>
                {
                    Queue.OnProgress?.Invoke(Queue, args);
                });
            }
        }

        public static ResourceQueue Shared = new ResourceQueue();

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private readonly List<Item> queue = new List<Item>();

        private Item currentItem;
        private CancellationTokenSource tokenSource;
        private bool isBusy = false;

        public event EventHandler<Item> OnEnqueue;
        public event EventHandler<Item> OnStart;
        public event EventHandler<ProgressEventArgs> OnProgress;
        public event EventHandler<Item> OnFinish;
        public event EventHandler<ErrorEventArgs> OnError;

        public Item[] Queue
        {
            get
            {
                return queue.ToArray();
            }
        }

        public void Enqueue(Item item)
        {
            queue.Add(item);
            OnEnqueue?.Invoke(this, item);

            var thread = new Thread(Dequeue);
            thread.IsBackground = true;
            thread.Start();
        }

        private async void Dequeue()
        {
            if (isBusy || queue.Count == 0) return;

            isBusy = true;

            var item = queue[0];
            queue.RemoveAt(0);

            dispatcher.Invoke(() =>
            {
                OnStart?.Invoke(this, item);
            });

            currentItem = item;
            tokenSource = new CancellationTokenSource();

            try
            {
                await Process(item);

                dispatcher.Invoke(() =>
                {
                    OnFinish?.Invoke(this, item);
                });
            }
            catch (Exception ex)
            {
                dispatcher.Invoke(() =>
                {
                    OnError?.Invoke(this, new ErrorEventArgs(item, ex));
                });
            }

            currentItem = null;
            tokenSource.Dispose();
            tokenSource = null;

            isBusy = false;

            Dequeue();
        }

        private async Task Process(Item item)
        {
            switch (item.Action)
            {
                case Item.ActionType.Download:
                    var client1 = await item.Source.Client.Clone();
                    await client1.Download(item.Source.Path, item.Destination.Path, new WorkerProgressReporter(item, this), tokenSource.Token);
                    await client1.Diconnect();
                    break;

                case Item.ActionType.Upload:
                    var client2 = await item.Destination.Client.Clone();
                    await client2.Upload(item.Destination.Path, item.Source.Path, new WorkerProgressReporter(item, this), tokenSource.Token);
                    await client2.Diconnect();
                    break;

                case Item.ActionType.Delete:
                    var client3 = await item.Source.Client.Clone();
                    await client3.Delete(item.Source.Path);
                    await client3.Diconnect();
                    break;
            }

            dispatcher.Invoke(() =>
            {
                item.ViewModel.Refresh();
            });

            tokenSource.Token.ThrowIfCancellationRequested();
        }

        public void Cancel(Item item)
        {
            queue.Remove(item);

            if (currentItem == item && tokenSource != null)
            {
                tokenSource.Cancel();
            }
        }
    }
}
