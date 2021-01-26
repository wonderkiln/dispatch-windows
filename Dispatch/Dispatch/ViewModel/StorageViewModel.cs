using Dispatch.Helpers;
using Dispatch.Service.Storage;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Dispatch.ViewModel
{
    public class StorageViewModel<T> : Observable
    {
        private readonly Storage<T[]> storage = new Storage<T[]>();

        public string FileName { get; private set; }

        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        public StorageViewModel(string fileName)
        {
            FileName = fileName;

            try
            {
                var items = storage.Load(FileName);

                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                storage.Save(Items.ToArray(), FileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
