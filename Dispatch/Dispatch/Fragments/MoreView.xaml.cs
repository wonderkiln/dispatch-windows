using Dispatch.View.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Fragments
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
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = Window.GetWindow(this);
            aboutWindow.DataContext = aboutWindow.Owner.DataContext;

            aboutWindow.ShowDialog();
        }
    }
}
