using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dispatch.Controls
{
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    public class DPTabListBoxItem : ListBoxItem
    {
        public event EventHandler<EventArgs> OnClose;

        public DPTabListBoxItem()
        {
            DefaultStyleKey = typeof(DPTabListBoxItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var closeButton = (Button)GetTemplateChild("PART_CloseButton");
            closeButton.Click += CloseButton_Click;

            MouseDown += TabListBoxItem_MouseDown;
        }

        private void TabListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                OnClose?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            OnClose?.Invoke(this, EventArgs.Empty);
        }
    }

    [TemplatePart(Name = "PART_AddButton", Type = typeof(Button))]
    public class DPTabListBox : ListBox
    {
        public event EventHandler<EventArgs> OnClose;
        public event EventHandler<EventArgs> OnAdd;

        public DPTabListBox()
        {
            DefaultStyleKey = typeof(DPTabListBox);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var closeButton = (Button)GetTemplateChild("PART_AddButton");
            closeButton.Click += AddButton_Click;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            OnAdd.Invoke(this, EventArgs.Empty);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DPTabListBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DPTabListBoxItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var listItem = (DPTabListBoxItem)element;
            listItem.OnClose += ListItem_OnClose;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            var listItem = (DPTabListBoxItem)element;
            listItem.OnClose -= ListItem_OnClose;
        }

        private void ListItem_OnClose(object sender, EventArgs e)
        {
            OnClose.Invoke(sender, EventArgs.Empty);
        }
    }
}
