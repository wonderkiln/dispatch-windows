using Dispatch.Properties;
using System;
using System.Windows;

namespace Dispatch
{
    public partial class App : Application
    {
        public enum Theme { Light, Dark }

        public static void ChangeTheme(Theme theme)
        {
            switch (theme)
            {
                case Theme.Light:
                    Current.Resources.MergedDictionaries[0].Source = new Uri("Theme/Colors.Light.xaml", UriKind.Relative);
                    break;

                case Theme.Dark:
                    Current.Resources.MergedDictionaries[0].Source = new Uri("Theme/Colors.Dark.xaml", UriKind.Relative);
                    break;
            }

            Settings.Default.DarkTheme = theme == Theme.Dark;
            Settings.Default.Save();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ChangeTheme(Settings.Default.DarkTheme ? Theme.Dark : Theme.Light);
        }
    }
}
