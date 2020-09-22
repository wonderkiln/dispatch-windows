using Dispatch.Client;
using Dispatch.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dispatch.ViewModel
{
    public class TransferObject : Observable
    {
        public enum TransferStatus { Queued, IntermitentProgress, InProgress, Finished, Cancelled, Error }

        private TransferStatus status = TransferStatus.Queued;
        public TransferStatus Status
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

        private double progress = -1;
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

        public string Error { get; set; }

        public IResource Source { get; set; }

        public IResource Destination { get; set; }

        public ICommand CancelCommand { get; set; }

        private CancellationTokenSource token;

        public TransferObject()
        {
            CancelCommand = new RelayCommand(() =>
            {
                token?.Cancel();
            });
        }

        public async void StartTransfer()
        {
            var draggedResource = Source;
            var droppedResource = Destination;

            var draggedClient = draggedResource.Client;
            var droppedClient = droppedResource.Client;

            Status = TransferStatus.IntermitentProgress;

            token = new CancellationTokenSource();

            if (draggedClient is LocalClient)
            {
                droppedClient.OnProgressChange += DroppedClient_OnProgressChange;

                try
                {
                    if (draggedResource.Directory)
                    {
                        await droppedClient.UploadDirectory(draggedResource.Path, droppedResource.CombinePath(draggedResource.Name), token.Token);
                    }
                    else
                    {
                        await droppedClient.UploadFile(draggedResource.Path, droppedResource.CombinePath(draggedResource.Name), token.Token);
                    }

                    Status = TransferStatus.Finished;
                }
                catch (Exception ex)
                {
                    Status = TransferStatus.Error;
                    Error = ex.Message;
                }
                finally
                {
                    droppedClient.OnProgressChange -= DroppedClient_OnProgressChange;
                }
            }
            else
            {
                draggedClient.OnProgressChange += DroppedClient_OnProgressChange;

                try
                {
                    if (draggedResource.Directory)
                    {
                        await draggedClient.DownloadDirectory(draggedResource.Path, droppedResource.CombinePath(draggedResource.Name), token.Token);
                    }
                    else
                    {
                        await draggedClient.DownloadFile(draggedResource.Path, droppedResource.CombinePath(draggedResource.Name), token.Token);
                    }

                    Status = TransferStatus.Finished;
                }
                catch (Exception ex)
                {
                    Status = TransferStatus.Error;
                    Error = ex.Message;
                }
                finally
                {
                    draggedClient.OnProgressChange -= DroppedClient_OnProgressChange;
                }
            }
        }

        private void DroppedClient_OnProgressChange(object sender, ClientProgress e)
        {
            Status = TransferStatus.InProgress;
            Progress = e.TotalProgress;
        }
    }

    public class QueueViewModel : Observable
    {
        public ObservableCollection<TransferObject> List { get; } = new ObservableCollection<TransferObject>();

        public void Add(IResource source, IResource destination)
        {
            var item = new TransferObject()
            {
                Source = source,
                Destination = destination,
            };

            List.Add(item);

            item.StartTransfer();
        }
    }
}
