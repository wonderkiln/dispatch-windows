using Dispatch.Helpers;
using Dispatch.Service.Model;
using Dispatch.Service.Storage;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Dispatch.ViewModel
{
    public class FavoritesViewModel : Observable
    {
        private static readonly string PATH = "favorites.json";

        private readonly Storage<FavoriteItem[]> storage = new Storage<FavoriteItem[]>();

        public ObservableCollection<FavoriteItem> Items { get; } = new ObservableCollection<FavoriteItem>();

        public FavoritesViewModel()
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

        public void Remove(FavoriteItem item)
        {
            Items.Remove(item);
        }
    }
}
