
using Dispatch.Helpers;
using Dispatch.Service.Model;
using System;
using System.Windows;

namespace Dispatch
{
    public partial class App : Application
    {
        private readonly WindowsThemeWatcher themeWatcher = new WindowsThemeWatcher();

        public void ChangeTheme(AppTheme theme)
        {
            switch (theme)
            {
                case AppTheme.Light:
                    Current.Resources.MergedDictionaries[0].Source = new Uri("Styles/Brushes.Light.xaml", UriKind.Relative);
                    break;

                case AppTheme.Dark:
                    Current.Resources.MergedDictionaries[0].Source = new Uri("Styles/Brushes.Dark.xaml", UriKind.Relative);
                    break;
            }
        }

        public void SetTheme(AppTheme theme)
        {
            var settings = WindowHelper.SettingsStorage.Load();
            settings.Theme = theme;
            WindowHelper.SettingsStorage.Save(settings);

            if (theme == AppTheme.Auto)
            {
                themeWatcher.OnChangeWindowsTheme += ThemeWatcher_OnChangeWindowsTheme;
            }
            else
            {
                themeWatcher.OnChangeWindowsTheme -= ThemeWatcher_OnChangeWindowsTheme;
                ChangeTheme(theme);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var settings = WindowHelper.SettingsStorage.Load();

            if (settings.Theme == AppTheme.Auto)
            {
                themeWatcher.OnChangeWindowsTheme += ThemeWatcher_OnChangeWindowsTheme;
            }
            else
            {
                ChangeTheme(settings.Theme);
            }
        }

        private void ThemeWatcher_OnChangeWindowsTheme(object sender, WindowsThemeWatcher.WindowsTheme e)
        {
            ChangeTheme(e == WindowsThemeWatcher.WindowsTheme.Light ? AppTheme.Light : AppTheme.Dark);
        }
    }
}
