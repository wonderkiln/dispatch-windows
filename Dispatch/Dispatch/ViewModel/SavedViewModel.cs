using Dispatch.Helpers;
using Dispatch.Service.Model;
using Dispatch.Service.Storage;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Dispatch.ViewModel
{
    public class SavedViewModel : Observable
    {
        private static readonly string PATH = "saved.json";

        private readonly Storage<SaveItem[]> storage = new Storage<SaveItem[]>();

        public ObservableCollection<SaveItem> Items { get; } = new ObservableCollection<SaveItem>();

        public SavedViewModel()
        {
            try
            {
                var items = storage.Load(PATH);

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
                storage.Save(Items.ToArray(), PATH);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
