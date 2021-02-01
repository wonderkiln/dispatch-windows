using Dispatch.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dispatch.Controls
{
    public class DPTabListBoxItem : ListBoxItem
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(DPTabListBoxItem), new PropertyMetadata(true));
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public DPTabListBoxItem()
        {
            DefaultStyleKey = typeof(DPTabListBoxItem);
        }
    }

    public class DPTabListBox : ListBox
    {
        public static readonly DependencyProperty AddTabCommandProperty = DependencyProperty.Register("AddTabCommand", typeof(ICommand), typeof(DPTabListBox));
        public ICommand AddTabCommand
        {
            get { return (ICommand)GetValue(AddTabCommandProperty); }
            set { SetValue(AddTabCommandProperty, value); }
        }

        public static readonly DependencyProperty RemoveTabCommandProperty = DependencyProperty.Register("RemoveTabCommand", typeof(ICommand), typeof(DPTabListBox));
        public ICommand RemoveTabCommand
        {
            get { return (ICommand)GetValue(RemoveTabCommandProperty); }
            set { SetValue(RemoveTabCommandProperty, value); }
        }

        public static readonly Duration AnimationDuration = new Duration(TimeSpan.FromMilliseconds(180));

        public ICommand CloseTabCommand { get; }

        public DPTabListBox()
        {
            CloseTabCommand = new RelayCommand<object>(CloseTab);
            DefaultStyleKey = typeof(DPTabListBox);
        }

        private async void CloseTab(object parameter)
        {
            var item = ItemContainerGenerator.ContainerFromItem(parameter) as DPTabListBoxItem;

            if (item != null && item.IsOpen)
            {
                item.IsOpen = false;

                // Wait for the close animation to finish
                await Task.Delay(AnimationDuration.TimeSpan);

                RemoveTabCommand?.Execute(parameter);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DPTabListBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DPTabListBoxItem;
        }
    }
}
