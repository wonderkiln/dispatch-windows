using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dispatch.Controls
{
    public class DPTabListBoxItem : ListBoxItem
    {
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

        public static readonly DependencyProperty CloseTabCommandProperty = DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(DPTabListBox));
        public ICommand CloseTabCommand
        {
            get { return (ICommand)GetValue(CloseTabCommandProperty); }
            set { SetValue(CloseTabCommandProperty, value); }
        }

        public DPTabListBox()
        {
            DefaultStyleKey = typeof(DPTabListBox);
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
