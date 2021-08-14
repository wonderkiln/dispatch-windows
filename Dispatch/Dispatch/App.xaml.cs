using Dispatch.Helpers;
using Dispatch.Service.Models;
using Dispatch.Service.Theme;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.IO;
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
            var settings = WindowHelper.SettingsStorage.Load(new Settings());
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

        public void SetFontSize(AppFontSize fontSize)
        {
            var settings = WindowHelper.SettingsStorage.Load(new Settings());
            settings.FontSize = fontSize;
            WindowHelper.SettingsStorage.Save(settings);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppCenter.Start("***REMOVED***", typeof(Analytics), typeof(Crashes));

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var settings = WindowHelper.SettingsStorage.Load(new Settings());

            if (settings.Theme == AppTheme.Auto)
            {
                themeWatcher.OnChangeWindowsTheme += ThemeWatcher_OnChangeWindowsTheme;
            }
            else
            {
                ChangeTheme(settings.Theme);
            }

            if (settings.IconThemePath == null)
            {
                settings.IconThemePath = Path.Combine(Directory.GetCurrentDirectory(), "Themes", "fluent.zip");
                WindowHelper.SettingsStorage.Save(settings);
            }

            try
            {
                FileIconTheme.LoadTheme(settings.IconThemePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ThemeWatcher_OnChangeWindowsTheme(object sender, WindowsThemeWatcher.WindowsTheme e)
        {
            ChangeTheme(e == WindowsThemeWatcher.WindowsTheme.Light ? AppTheme.Light : AppTheme.Dark);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
