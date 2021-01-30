using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ChangeThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to restart and apply the theme?", "Restart required", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                App.ToggleTheme();
                Application.Current.Shutdown();
                Process.Start(Application.ResourceAssembly.Location);
            }
        }
    }
}
