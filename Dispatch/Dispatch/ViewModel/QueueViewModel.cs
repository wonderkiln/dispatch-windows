using Dispatch.Client;
using Dispatch.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.ViewModel
{
    public class TransferObject : Observable
    {
        public enum TransferStatus { Queued, InProgress, Finished, Cancelled, Error }

        public TransferStatus Status { get; set; } = TransferStatus.Queued;

        public double Progress { get; set; }

        public string Error { get; set; }

        public Resource Source { get; set; }

        public Resource Destination { get; set; }
    }

    public class QueueViewModel : Observable
    {
        ObjectQueue<TransferObject> queue = new ObjectQueue<TransferObject>();

        public List<TransferObject> List
        {
            get
            {
                return queue.List;
            }
        }

        public void Add(Resource source, Resource destination)
        {
            var obj = new TransferObject()
            {
                Source = source,
                Destination = destination,
            };

            queue.Enqueue(obj, new Action<TransferObject, Action>(async (item, callback) =>
            {
                //await Task.Delay(1000);
                var path = await item.Source.Client.Download(item.Source, Path.GetTempPath());
                await item.Destination.Client.Upload(path, item.Destination.Path);
                callback();
            }));

            Notify("List");
        }
    }
}
