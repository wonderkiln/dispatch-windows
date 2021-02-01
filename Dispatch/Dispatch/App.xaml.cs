
using Dispatch.Helpers;
using Dispatch.Service.Model;
using System;
using System.Windows;

namespace Dispatch
{
    public partial class App : Application
    {
        public static void ChangeTheme(AppTheme theme)
        {
            switch (theme)
            {
                case AppTheme.Light:
                    Current.Resources.MergedDictionaries[0].Source = new Uri("Styles/Colors.Light.xaml", UriKind.Relative);
                    break;

                case AppTheme.Dark:
                    Current.Resources.MergedDictionaries[0].Source = new Uri("Styles/Colors.Dark.xaml", UriKind.Relative);
                    break;
            }
        }

        public static void ToggleTheme()
        {
            var settings = WindowHelper.SettingsStorage.Load();
            settings.Theme = settings.Theme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
            ChangeTheme(settings.Theme);
            WindowHelper.SettingsStorage.Save(settings);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var settings = WindowHelper.SettingsStorage.Load();
            ChangeTheme(settings.Theme);
        }
    }
}
