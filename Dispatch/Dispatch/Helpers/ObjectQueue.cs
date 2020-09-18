using Dispatch.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Helpers
{
    public class ObjectQueue<T>
    {
        private ObservableCollection<Tuple<T, Action<T, Action>>> list = new ObservableCollection<Tuple<T, Action<T, Action>>>();

        public List<T> List
        {
            get
            {
                return list.Select(e => e.Item1).ToList();
            }
        }

        private bool working = false;

        public ObjectQueue()
        {
            list.CollectionChanged += List_CollectionChanged;
        }

        private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //
        }

        public void Enqueue(T item, Action<T, Action> action)
        {
            list.Add(new Tuple<T, Action<T, Action>>(item, action));
            Dequeue();
        }

        private void Dequeue()
        {
            if (list.Count == 0 || working) { return; }

            working = true;

            var item = list[0];
            list.RemoveAt(0);

            item.Item2(item.Item1, new Action(() =>
            {
                working = false;
                Dequeue();
            }));
        }
    }
}
