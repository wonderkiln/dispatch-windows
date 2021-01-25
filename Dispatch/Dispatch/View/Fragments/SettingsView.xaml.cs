using Dispatch.View.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class SettingsView : UserControl
    {
        private Action closeSidebar;

        public SettingsView(Action closeSidebar)
        {
            InitializeComponent();
            this.closeSidebar = closeSidebar;
        }

        private void ChangeThemeButton_Click(object sender, RoutedEventArgs e)
        {
            App.ToggleTheme();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            closeSidebar?.Invoke();
            new AboutWindow() { Owner = Window.GetWindow(this) }.ShowDialog();
        }
    }
}
