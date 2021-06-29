using Dispatch.Helpers;
using System.Windows;

namespace Dispatch
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.EnableBlurForWindow(this);
        }
    }
}
