using Dispatch.View.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class MoreView : UserControl
    {
        public Action CloseSidebar { get; set; }
        public Action<string, object> ChangeSidebar { get; set; }

        public MoreView()
        {
            InitializeComponent();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeSidebar?.Invoke("Settings", new SettingsView());
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            CloseSidebar?.Invoke();
            new AboutWindow() { Owner = Window.GetWindow(this) }.ShowDialog();
        }
    }
}
