using Dispatch.Helpers;
using System.Windows;

namespace Dispatch
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.EnableBlurForWindow(this);
        }
    }
}
