using Dispatch.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Helpers
{
    public class ResourceWatcher
    {
        public static ResourceWatcher Instance = new ResourceWatcher();

        public Dictionary<string, IResource> Resources { get; set; } = new Dictionary<string, IResource>();

        public IResource Find(IResource resource)
        {
            return Resources.Values.FirstOrDefault(e => e.Path == resource.Path && e.Client == resource.Client);
        }

        public IResource Find(string localPath)
        {
            return Resources[localPath];
        }

        public void Watch(IResource resource, string localPath)
        {
            if (Find(resource) == null)
            {
                Resources.Add(localPath, resource);

                var watcher = new FileSystemWatcher();
                watcher.Changed += Watcher_Changed;
                watcher.Deleted += Watcher_Deleted;
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.IncludeSubdirectories = false;
                watcher.Filter = Path.GetFileName(localPath);
                watcher.Path = Path.GetDirectoryName(localPath);
                watcher.EnableRaisingEvents = true;
            }
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            var watcher = (FileSystemWatcher)sender;
            watcher.Dispose();
        }

        private async void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var resource = Find(e.FullPath);

            if (resource != null)
            {
                await resource.Client.UploadFile(e.FullPath, resource.Path);
            }
        }
    }
}
