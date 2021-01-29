using Dispatch.View.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class MoreView : UserControl
    {
        public event EventHandler<object> OnChangeSidebar;

        public MoreView()
        {
            InitializeComponent();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            OnChangeSidebar?.Invoke(this, new SettingsView());
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow() { Owner = Window.GetWindow(this) }.ShowDialog();
        }
    }
}
