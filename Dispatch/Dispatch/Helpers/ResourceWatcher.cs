using Dispatch.Service.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Dispatch.Helpers
{
    public class ResourceWatcher : IProgress<ResourceProgress>
    {
        public class Item
        {
            public string Path { get; private set; }

            public FileSystemWatcher Watcher { get; private set; }

            public Resource Resource { get; private set; }

            public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

            public Item(string path, FileSystemWatcher watcher, Resource resource)
            {
                Path = path;
                Watcher = watcher;
                Resource = resource;
            }
        }

        public static ResourceWatcher Shared = new ResourceWatcher();

        private List<Item> resources = new List<Item>();

        public void Watch(Resource resource, string path)
        {
            if (resources.Find(e => e.Path == path) == null)
            {
                var watcher = new FileSystemWatcher();
                watcher.Changed += Watcher_Changed;
                watcher.Deleted += Watcher_Deleted;
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.IncludeSubdirectories = false;
                watcher.Filter = Path.GetFileName(path);
                watcher.Path = Path.GetDirectoryName(path);
                watcher.EnableRaisingEvents = true;

                resources.Add(new Item(path, watcher, resource));
            }
        }

        public void Unwatch(string path)
        {
            var item = resources.Find(e => e.Path == path);

            if (item != null)
            {
                item.Watcher.Dispose();
                item.TokenSource.Cancel();
                resources.Remove(item);
            }
        }

        private async void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var item = resources.Find(i => i.Path == e.FullPath);

            if (item != null)
            {
                try
                {
                    await item.Resource.Client.Upload(item.Resource.Path, item.Path, this, item.TokenSource.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Unwatch(e.FullPath);
        }

        public void Report(ResourceProgress value)
        {
            Console.WriteLine(value.TotalProgress);
        }
    }
}
