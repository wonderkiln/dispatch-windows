using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Controls
{
    public partial class ChromeButtons : UserControl
    {
        public ChromeButtons()
        {
            InitializeComponent();
        }

        private Window window;

        private void Window_StateChanged(object sender, EventArgs e)
        {
            Restore.Visibility = window.WindowState != WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
            Maximize.Visibility = window.WindowState == WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;

            if (window.Content is FrameworkElement)
            {
                (window.Content as FrameworkElement).Margin = new Thickness(window.WindowState == WindowState.Maximized ? 6 : 0);
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(window);
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(window);
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(window);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(window);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            window.StateChanged += Window_StateChanged;
            Window_StateChanged(sender, e);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            window.StateChanged -= Window_StateChanged;
            window = null;
        }
    }
}
