using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void LightThemeButton_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeTheme(App.Theme.Light);
        }

        private void DarkThemeButton_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeTheme(App.Theme.Dark);
        }
    }
}
