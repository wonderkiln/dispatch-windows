﻿using Dispatch.Helpers;
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

        private readonly string fileName;

        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        public RelayCommand<T> AddCommand { get; }
        public RelayCommand<T> DeleteCommand { get; }

        public StorageViewModel(string fileName)
        {
            AddCommand = new RelayCommand<T>(Add);
            DeleteCommand = new RelayCommand<T>(Delete);

            this.fileName = fileName;

            try
            {
                var items = storage.Load(this.fileName);

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
                storage.Save(Items.ToArray(), fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Add(T item)
        {
            Items.Add(item);
        }

        public void Delete(T item)
        {
            Items.Remove(item);
        }
    }
}
