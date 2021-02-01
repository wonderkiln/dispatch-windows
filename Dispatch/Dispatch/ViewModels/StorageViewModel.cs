using Dispatch.Helpers;
using Dispatch.Service.Storage;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Dispatch.ViewModels
{
    public class StorageViewModel<T> : Observable
    {
        private readonly Storage<T[]> storage;

        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        public RelayCommand<T> AddCommand { get; }
        public RelayCommand<T> DeleteCommand { get; }

        public StorageViewModel(string fileName)
        {
            AddCommand = new RelayCommand<T>(Add);
            DeleteCommand = new RelayCommand<T>(Delete);

            storage = new Storage<T[]>(fileName);

            try
            {
                var items = storage.Load();

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
                storage.Save(Items.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Insert(int index, T item)
        {
            Items.Insert(index, item);
        }

        public void Add(T item)
        {
            Items.Add(item);
        }

        public void Delete(T item)
        {
            Items.Remove(item);
        }

        public void Move(int index, T item)
        {
            var oldIndex = Items.IndexOf(item);

            if (oldIndex == -1 || oldIndex == index) return;

            if (index >= 0 && index < Items.Count)
            {
                Items.Move(oldIndex, index);
            }
            else if (index >= Items.Count)
            {
                Items.RemoveAt(oldIndex);
                Items.Add(item);
            }
        }
    }
}
