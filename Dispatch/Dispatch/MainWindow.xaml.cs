using Dispatch.Helpers;
using System.ComponentModel;
using System.Windows;

namespace Dispatch.View.Windows
{
    public partial class MainWindow : Window
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.LoadWindowSettings(this);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            WindowHelper.SaveWindowSettings(this);
        }
    }
}
