using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Fragments
{
    public partial class MoreView : UserControl
    {
        public MoreView()
        {
            InitializeComponent();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = Window.GetWindow(this);

            settingsWindow.Show();
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
